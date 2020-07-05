using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Runtime.Internal;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Compiler
{
    public sealed class QsiTableCompiler
    {
        private const string scopeFieldList = "field list";

        public IQsiLanguageService LanguageService { get; }

        private readonly IQsiTreeParser _treeParser;
        private readonly IQsiScriptParser _scriptParser;
        private readonly IQsiReferenceResolver _resolver;

        public QsiTableCompiler(IQsiLanguageService languageService)
        {
            LanguageService = languageService;
            _treeParser = languageService.CreateTreeParser();
            _scriptParser = languageService.CreateScriptParser();
            _resolver = languageService.CreateResolver();
        }

        #region Execute
        public async Task<QsiTableResult> ExecuteAsync(IQsiTableNode tableNode)
        {
            using var scope = new CompileContext();
            var structure = await BuildTableStructure(scope, tableNode);

            return new QsiTableResult(structure, null);
        }

        public async Task<QsiTableResult> ExecuteAsync(QsiScript script)
        {
            try
            {
                if (script.ScriptType != QsiScriptType.Select)
                    throw Throw($"Not supported script type: {script.ScriptType}");

                var treeNode =
                    _treeParser.Parse(script) ??
                    throw Throw($"{nameof(IQsiTreeParser)}.{nameof(IQsiTreeParser.Parse)} result was null");

                if (!(treeNode is IQsiTableNode tableNode))
                    throw Throw($"Not supported qsi tree type: {treeNode.GetType().FullName}");

                return await ExecuteAsync(tableNode);
            }
            catch (Exception e)
            {
                return new QsiTableResult(null, new[] { e });
            }
        }

        public async IAsyncEnumerable<QsiTableResult> ExecuteAsync(string input)
        {
            foreach (var script in _scriptParser.Parse(input).Where(s => s.ScriptType == QsiScriptType.Select))
            {
                yield return await ExecuteAsync(script);
            }
        }
        #endregion

        private Task<QsiDataTable> BuildTableStructure(CompileContext context, IQsiTableNode table)
        {
            switch (table)
            {
                case IQsiTableAccessNode tableAccess:
                    return BuildTableAccessStructure(context, tableAccess);

                case IQsiDerivedTableNode derivedTable:
                    return BuildDerivedTableStructure(context, derivedTable);

                case IQsiJoinedTableNode joinedTable:
                    return BuildJoinedTable(context, joinedTable);

                case IQsiCompositeTableNode compositeTable:
                    return BuildCompositeTable(context, compositeTable);
            }

            throw new InvalidOperationException();
        }

        private async Task<QsiDataTable> BuildTableAccessStructure(CompileContext context, IQsiTableAccessNode table)
        {
            var lookup = ResolveDataTable(context, table.Identifier);

            // view
            if (lookup.Type == QsiDataTableType.View || lookup.Type == QsiDataTableType.MaterializedView)
            {
                var script = _resolver.LookupDefinition(lookup.Identifier, lookup.Type);
                var viewTable = (IQsiTableNode)_treeParser.Parse(script);

                using var viewCompileContext = new CompileContext();
                var viewTableStructure = await BuildTableStructure(viewCompileContext, viewTable);

                viewTableStructure.Identifier = _resolver.ResolveQualifiedIdentifier(viewTableStructure.Identifier);
                lookup = viewTableStructure;
            }

            // push table reference
            context.PushTable(lookup);

            return lookup;
        }

        private async Task<QsiDataTable> BuildDerivedTableStructure(CompileContext context, IQsiDerivedTableNode table)
        {
            using var scopedContext = new CompileContext(context);

            // Directives

            if (table.Directives?.Tables?.Length > 0)
            {
                foreach (var directive in table.Directives.Tables)
                {
                    using var directiveContext = new CompileContext(context);
                    var directiveTable = await BuildTableStructure(directiveContext, directive);

                    scopedContext.AddDirective(directiveTable);
                }
            }

            // Table Source

            var alias = table.Alias?.Name;

            if (table.Source != null)
            {
                using var sourceContext = new CompileContext(scopedContext);
                var sourceTable = await BuildTableStructure(sourceContext, table.Source);
                scopedContext.PushTable(sourceTable);
            }

            var declaredTable = new QsiDataTable
            {
                Type = QsiDataTableType.Derived,
                Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
            };

            if (table.Columns == null || table.Columns.Count == 0)
                throw ThrowCheckSyntax();

            // Columns Definition

            foreach (var column in table.Columns.Columns)
            {
                IEnumerable<QsiDataColumn> columns = ResolveColumns(scopedContext, column);

                if (column is IQsiDerivedColumnNode derivedColum)
                {
                    var declaredColumn = declaredTable.NewColumn();
                    declaredColumn.Name = derivedColum.Alias?.Name;
                    declaredColumn.IsExpression = derivedColum.IsExpression;

                    declaredColumn.References.AddRange(columns);
                }
                else
                {
                    foreach (var c in columns)
                    {
                        var declaredColumn = declaredTable.NewColumn();
                        declaredColumn.Name = c.Name;
                        declaredColumn.References.Add(c);
                    }
                }
            }

            // push table reference
            if (declaredTable.HasIdentifier)
            {
                context.PushTable(declaredTable);
            }

            return declaredTable;
        }

        private Task<QsiDataTable> BuildJoinedTable(CompileContext context, IQsiJoinedTableNode table)
        {
            throw new NotImplementedException();
        }

        private async Task<QsiDataTable> BuildCompositeTable(CompileContext context, IQsiCompositeTableNode table)
        {
            if (table.Sources == null || table.Sources.Length == 0)
                throw ThrowCheckSyntax();

            QsiDataTable[] sources = await Task.WhenAll(table.Sources.Select(s => BuildTableStructure(context, s)));

            int columnCount = sources[0].Columns.Count;

            if (sources.Skip(1).Any(s => s.Columns.Count != columnCount))
                throw Throw("The used Statements have a different number of columns.");

            var compositeSource = new QsiDataTable
            {
                Type = QsiDataTableType.Union
            };

            for (int i = 0; i < columnCount; i++)
            {
                var baseColumn = sources[0].Columns[i];
                var compositeColumn = compositeSource.NewColumn();

                compositeColumn.Name = baseColumn.Name;
                compositeColumn.References.AddRange(sources.Select(s => s.Columns[i]));
            }

            return compositeSource;
        }

        #region Column Lookup
        private IEnumerable<QsiDataColumn> ResolveColumns(CompileContext context, IQsiColumnNode column)
        {
            switch (column)
            {
                case IQsiAllColumnNode allColumn:
                    return ResolveAllColumns(context, allColumn);

                case IQsiDeclaredColumnNode declaredColumn:
                    return new[] { ResolveDeclaredColumn(context, declaredColumn.Name) };

                case IQsiDerivedColumnNode derivedColumn:
                    return ResolveDerivedColumns(context, derivedColumn);

                case IQsiSequentialColumnNode sequentialColumn:
                    return new[] { ResolveSequentialColumn(context, sequentialColumn) };
            }

            throw new InvalidOperationException();
        }

        private IEnumerable<QsiDataColumn> ResolveAllColumns(CompileContext context, IQsiAllColumnNode column)
        {
            // *
            if (column.Path == null)
            {
                var source = context.PeekTable();

                if (source == null)
                    throw ThrowNoTablesUsed();

                return source.Columns;
            }

            // path.or.alias.*

            QsiDataTable[] tables = LookupDataTablesInExpression(context, column.Path).ToArray();

            if (tables.Length == 0)
                throw ThrowUnknownTable(column.Path);

            return tables.SelectMany(t => t.Columns);
        }

        private QsiDataColumn ResolveDeclaredColumn(CompileContext context, QsiQualifiedIdentifier column)
        {
            IEnumerable<QsiDataTable> sources;

            if (column.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(column.Identifiers[..^1]);
                sources = LookupDataTablesInExpression(context, identifier).ToArray();

                if (!sources.Any())
                    throw ThrowUnknownTableIn(identifier, scopeFieldList);
            }
            else if (column.Level == 0)
            {
                throw new InvalidOperationException();
            }
            else
            {
                sources = context.Tables;
            }

            var columnName = column.Identifiers[^1];

            QsiDataColumn[] columns = sources
                .SelectMany(s => s.Columns.Where(c => Match(c.Name, columnName)))
                .Take(2)
                .ToArray();

            if (columns.Length == 0)
                throw ThrowUnknownColumnIn(columnName.Value, scopeFieldList);

            if (columns.Length > 1)
                throw ThrowAmbiguousColumnIn(column, scopeFieldList);

            return columns[0];
        }

        private IEnumerable<QsiDataColumn> ResolveDerivedColumns(CompileContext context, IQsiDerivedColumnNode column)
        {
            if (column.IsExpression)
                return InspectColumnsInExpression(context, column.Expression);

            return ResolveColumns(context, column.Column);
        }

        private QsiDataColumn ResolveSequentialColumn(CompileContext context, IQsiSequentialColumnNode column)
        {
            var table = context.PeekTable();

            if (table == null)
                throw ThrowNoTablesUsed();

            return table.Columns[column.Ordinal];
        }
        #endregion

        #region Table Lookup
        private QsiDataTable ResolveDataTable(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            return LookupDataTable(context, identifier) ?? throw ThrowUnableResolveTable(identifier);
        }

        private QsiDataTable LookupDataTable(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            return
                context.Directives.FirstOrDefault(d => Match(d.Identifier, identifier)) ??
                _resolver.LookupTable(_resolver.ResolveQualifiedIdentifier(identifier));
        }

        private IEnumerable<QsiDataTable> LookupDataTablesInExpression(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            foreach (var table in context.Tables.Where(t => t.HasIdentifier))
            {
                // * case - Exact access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT sakila.actor.column FROM sakila.actor             │
                // │        ▔▔▔▔▔▔^▔▔▔▔▔      ==     ▔▔▔▔▔▔^▔▔▔▔▔             │
                // │         └-> identifier(2)        └-> table.Identifier(2) │
                // └──────────────────────────────────────────────────────────┘ 

                if (Match(table.Identifier, identifier))
                    yield return table;

                // * case - 2 Level implied access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT actor.column FROM sakila.actor                    │
                // │        ▔▔▔▔▔      <       ▔▔▔▔▔^▔▔▔▔▔                    │
                // │         └-> identifier(1)  └-> table.Identifier(2)       │
                // └──────────────────────────────────────────────────────────┘ 

                // * case - 3 Level implied access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT sakila.actor.column FROM db.sakila.actor          │
                // │        ▔▔▔▔▔▔^▔▔▔▔▔       <     ▔▔^▔▔▔▔▔▔^▔▔▔▔▔          │
                // │         └-> identifier(2)        └-> table.Identifier(3) │
                // └──────────────────────────────────────────────────────────┘ 

                if (!IsReferenceType(table.Type))
                    continue;

                if (table.Identifier.Level <= identifier.Level)
                    continue;

                QsiIdentifier[] partialIdentifiers = table.Identifier.Identifiers[^identifier.Level..];
                var partialIdentifier = new QsiQualifiedIdentifier(partialIdentifiers);

                if (Match(partialIdentifier, identifier))
                    yield return table;
            }
        }
        #endregion

        #region Expression
        private IEnumerable<QsiDataColumn> InspectColumnsInExpression(CompileContext context, IQsiExpressionNode expression)
        {
            if (expression == null)
                yield break;

            switch (expression)
            {
                case IQsiAssignExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Variable))
                        yield return c;

                    foreach (var c in InspectColumnsInExpression(context, e.Value))
                        yield return c;

                    break;
                }

                case IQsiInvokeExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Member))
                        yield return c;

                    foreach (var c in InspectColumnsInExpression(context, e.Parameters))
                        yield return c;

                    break;
                }

                case IQsiLiteralExpressionNode e:
                {
                    break;
                }

                case IQsiLogicalExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Left))
                        yield return c;

                    foreach (var c in InspectColumnsInExpression(context, e.Right))
                        yield return c;

                    break;
                }

                case IQsiParametersExpressionNode e:
                {
                    foreach (var c in e.Expressions.SelectMany(x => InspectColumnsInExpression(context, x)))
                        yield return c;

                    break;
                }

                case IQsiSwitchExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Value))
                        yield return c;

                    foreach (var c in e.Cases.SelectMany(c => InspectColumnsInExpression(context, c)))
                        yield return c;

                    break;
                }

                case IQsiSwitchCaseExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Condition))
                        yield return c;

                    foreach (var c in InspectColumnsInExpression(context, e.Consequent))
                        yield return c;

                    break;
                }

                case IQsiTableExpressionNode e:
                {
                    using var scopedContext = new CompileContext(context);
                    var structure = BuildTableStructure(scopedContext, e.Table).Result;

                    foreach (var c in structure.Columns)
                        yield return c;

                    break;
                }

                case IQsiUnaryExpressionNode e:
                {
                    foreach (var c in InspectColumnsInExpression(context, e.Expression))
                        yield return c;

                    break;
                }

                case IQsiColumnAccessExpressionNode e:
                {
                    yield return ResolveDeclaredColumn(context, e.Identifier);

                    break;
                }

                case IQsiVariableAccessExpressionNode e:
                {
                    // TODO: Analyze variable
                    break;
                }

                case IQsiFunctionAccessExpressionNode e:
                {
                    // TODO: Analyze function
                    break;
                }

                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion

        #region Misc
        private bool Match(QsiIdentifier a, QsiIdentifier b)
        {
            return LanguageService.MatchIdentifier(a, b);
        }
        
        private bool Match(QsiQualifiedIdentifier a, QsiQualifiedIdentifier b)
        {
            if (a.Level != b.Level)
                return false;

            for (int i = 0; i < a.Level; i++)
            {
                if (!Match(a.Identifiers[i], b.Identifiers[i]))
                    return false;
            }

            return true;
        }

        private static bool IsReferenceType(QsiDataTableType type)
        {
            return
                type == QsiDataTableType.Table ||
                type == QsiDataTableType.View ||
                type == QsiDataTableType.MaterializedView;
        }

        private static QsiDataTable WrapDataTable(QsiDataTable dataTable, QsiIdentifier identifier)
        {
            var table = new QsiDataTable
            {
                Type = dataTable.Type,
                Identifier = new QsiQualifiedIdentifier(identifier)
            };

            foreach (var column in dataTable.Columns)
            {
                var newColumn = table.NewColumn();

                newColumn.Name = column.Name;
                newColumn._isExpression = column._isExpression;
                newColumn.References.Add(column);
            }

            return table;
        }
        #endregion

        #region Exceptions
        private static QsiException ThrowCheckSyntax()
        {
            return Throw("You have an error in your SQL syntax; check the manual that corresponds to your Database server version");
        }

        private static QsiException ThrowUnableResolveTable(QsiQualifiedIdentifier identifier)
        {
            return Throw($"Unable to resolve table '{identifier}'");
        }

        private static QsiException ThrowUnableResolveColumn(string name)
        {
            return Throw($"Unable to resolve column '{name}'");
        }

        private static QsiException ThrowUnknownTable(QsiQualifiedIdentifier identifier)
        {
            return Throw($"Unknown table '{identifier}'");
        }

        private static QsiException ThrowUnknownTableIn(QsiQualifiedIdentifier identifier, string scope)
        {
            return Throw($"Unknown table '{identifier}' in {scope}");
        }

        private static QsiException ThrowUnknownViewIn(QsiQualifiedIdentifier identifier, string scope)
        {
            return Throw($"Unknown view '{identifier}' in {scope}");
        }

        private static QsiException ThrowUnknownColumnIn(string name, string scope)
        {
            return Throw($"Unknown column '{name}' in {scope}");
        }

        private static QsiException ThrowAmbiguousColumnIn(QsiQualifiedIdentifier identifier, string scope)
        {
            return Throw($"Column '{identifier}' in {scope} is ambiguous");
        }

        private static QsiException ThrowNoTablesUsed()
        {
            return Throw("No tables used");
        }

        private static QsiException Throw(string message)
        {
            return new QsiException(message);
        }
        #endregion
    }
}
