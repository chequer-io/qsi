using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Compiler.Proxy;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Parsing;
using Qsi.Runtime.Internal;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Compiler
{
    public class QsiTableCompiler
    {
        private const string scopeFieldList = "field list";

        public IQsiLanguageService LanguageService { get; }

        private readonly IQsiTreeParser _treeParser;
        private readonly IQsiScriptParser _scriptParser;
        private readonly IQsiReferenceResolver _resolver;
        private readonly QsiTableCompileOptions _options;

        public QsiTableCompiler(IQsiLanguageService languageService)
        {
            LanguageService = languageService;
            _treeParser = languageService.CreateTreeParser();
            _scriptParser = languageService.CreateScriptParser();
            _resolver = languageService.CreateResolver();
            _options = languageService.CreateCompileOptions() ?? new QsiTableCompileOptions();
        }

        #region Execute
        public async ValueTask<QsiTableResult> ExecuteAsync(IQsiTableNode tableNode)
        {
            using var scope = new CompileContext();
            var structure = await BuildTableStructure(scope, tableNode);

            return new QsiTableResult(structure, null);
        }

        public async ValueTask<QsiTableResult> ExecuteAsync(QsiScript script)
        {
            try
            {
                if (script.ScriptType != QsiScriptType.Select)
                    throw new QsiException(QsiError.NotSupportedTree, script.ScriptType);

                var treeNode =
                    _treeParser.Parse(script) ??
                    throw new QsiException(QsiError.Internal, $"{nameof(IQsiTreeParser)}.{nameof(IQsiTreeParser.Parse)} result was null");

                if (!(treeNode is IQsiTableNode tableNode))
                    throw new QsiException(QsiError.NotSupportedTree, treeNode.GetType().FullName);

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

        #region Table
        private async ValueTask<QsiDataTable> BuildTableStructure(CompileContext context, IQsiTableNode table)
        {
            switch (table)
            {
                case IQsiTableAccessNode tableAccess:
                    return await BuildTableAccessStructure(context, tableAccess);

                case IQsiDerivedTableNode derivedTable:
                    return await BuildDerivedTableStructure(context, derivedTable);

                case IQsiInlineDerivedTableNode inlineDerivedTableNode:
                    return await BuildInlineDerivedTableStructure(context, inlineDerivedTableNode);

                case IQsiJoinedTableNode joinedTable:
                    return await BuildJoinedTable(context, joinedTable);

                case IQsiCompositeTableNode compositeTable:
                    return await BuildCompositeTable(context, compositeTable);
            }

            throw new InvalidOperationException();
        }

        protected virtual async ValueTask<QsiDataTable> BuildTableAccessStructure(CompileContext context, IQsiTableAccessNode table)
        {
            var lookup = ResolveDataTable(context, table.Identifier);

            // view
            if (!lookup.IsSystem &&
                (lookup.Type == QsiDataTableType.View || lookup.Type == QsiDataTableType.MaterializedView))
            {
                var script = _resolver.LookupDefinition(lookup.Identifier, lookup.Type) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, lookup.Identifier);

                var viewTable = (IQsiTableNode)_treeParser.Parse(script) ??
                                throw new QsiException(QsiError.Syntax);

                var typeBackup = lookup.Type;

                using var viewCompileContext = new CompileContext();
                var viewTableStructure = await BuildTableStructure(viewCompileContext, viewTable);

                viewTableStructure.Identifier = _resolver.ResolveQualifiedIdentifier(viewTableStructure.Identifier);
                lookup = viewTableStructure;
                lookup.Type = typeBackup;
            }

            // // push table reference
            // context.PushTable(lookup);

            return lookup;
        }

        protected virtual async ValueTask<QsiDataTable> BuildDerivedTableStructure(CompileContext context, IQsiDerivedTableNode table)
        {
            using var scopedContext = new CompileContext(context);

            // Directives

            if (table.Directives?.Tables?.Length > 0)
            {
                await BuildDirectives(scopedContext, table.Directives);
            }

            // Table Source

            var alias = table.Alias?.Name;

            if (alias == null &&
                table.Parent is IQsiDerivedColumnNode &&
                !_options.AllowNoAliasInDerivedTable)
            {
                throw new QsiException(QsiError.NoAlias);
            }

            if (table.Source is IQsiJoinedTableNode joinedTableNode)
            {
                scopedContext.SourceTable = await BuildJoinedTable(scopedContext, joinedTableNode);
            }
            else if (table.Source != null)
            {
                using var sourceContext = new CompileContext(scopedContext);
                scopedContext.SourceTable = await BuildTableStructure(scopedContext, table.Source);
            }

            var declaredTable = new QsiDataTable
            {
                Type = QsiDataTableType.Derived,
                Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
            };

            if (table.Columns == null || table.Columns.Count == 0)
            {
                if (!_options.AllowEmptyColumnsInSelect)
                {
                    throw new QsiException(QsiError.Syntax);
                }
            }
            else
            {
                // Columns Definition

                foreach (var column in table.Columns.Columns)
                {
                    if (column is IQsiBindingColumnNode bindingColumn)
                    {
                        var declaredColumn = declaredTable.NewColumn();
                        declaredColumn.Name = new QsiIdentifier(bindingColumn.Id, false);
                        declaredColumn.IsBinding = true;
                        continue;
                    }

                    IEnumerable<QsiDataColumn> columns = ResolveColumns(scopedContext, column);

                    switch (column)
                    {
                        case IQsiDerivedColumnNode derivedColum:
                        {
                            var declaredColumn = declaredTable.NewColumn();

                            declaredColumn.Name = derivedColum.Alias?.Name;
                            declaredColumn.IsExpression = derivedColum.IsExpression;
                            declaredColumn.References.AddRange(columns);
                            break;
                        }

                        case IQsiSequentialColumnNode sequentialColum:
                        {
                            if (columns.Count() != 1)
                                throw new InvalidOperationException();

                            var c = columns.First();
                            var declaredColumn = declaredTable.NewColumn();

                            declaredColumn.Name = sequentialColum.Alias?.Name;
                            declaredColumn.References.Add(c);
                            break;
                        }

                        default:
                        {
                            foreach (var c in columns)
                            {
                                var declaredColumn = declaredTable.NewColumn();

                                declaredColumn.Name = c.Name;
                                declaredColumn.References.Add(c);
                            }

                            break;
                        }
                    }
                }
            }

            return declaredTable;
        }

        protected virtual ValueTask<QsiDataTable> BuildInlineDerivedTableStructure(CompileContext context, IQsiInlineDerivedTableNode table)
        {
            var alias = table.Alias?.Name;

            if (alias == null &&
                table.Parent is IQsiDerivedColumnNode &&
                !_options.AllowNoAliasInDerivedTable)
            {
                throw new QsiException(QsiError.NoAlias);
            }

            var declaredTable = new QsiDataTable
            {
                Type = QsiDataTableType.Inline,
                Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
            };

            IQsiSequentialColumnNode[] columns = table.Columns?.Columns
                .Cast<IQsiSequentialColumnNode>()
                .ToArray();

            int? columnCount = null;

            if (columns?.Length > 0)
            {
                foreach (var column in columns)
                {
                    var c = declaredTable.NewColumn();
                    c.Name = column.Alias.Name;
                }

                columnCount = columns.Length;
            }

            // Skip trace columns in expression.
            // Because don't know the possibility of declaring a referenceable column in the expression.
            // ISSUE: row.ColumnValues
            foreach (var row in table.Rows ?? Enumerable.Empty<IQsiRowValueExpressionNode>())
            {
                if (!columnCount.HasValue)
                {
                    columnCount = row.ColumnValues.Length;
                }
                else if (columnCount != row.ColumnValues.Length)
                {
                    throw new QsiException(QsiError.DifferentColumnsCount);
                }
            }

            if (!columnCount.HasValue)
            {
                if (!_options.AllowEmptyColumnsInInline)
                    throw new QsiException(QsiError.NoColumnsSpecified, alias);

                columnCount = 0;
            }

            if (declaredTable.Columns.Count != columnCount)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    declaredTable.NewColumn();
                }
            }

            return new ValueTask<QsiDataTable>(declaredTable);
        }

        protected virtual async ValueTask<QsiDataTable> BuildRecursiveCompositeTable(
            CompileContext context,
            IQsiDerivedTableNode table,
            IQsiCompositeTableNode source)
        {
            var declaredTable = new QsiDataTable
            {
                Type = QsiDataTableType.Derived,
                Identifier = new QsiQualifiedIdentifier(table.Alias.Name)
            };

            int sourceOffset = 0;
            var structures = new List<QsiDataTable>(source.Sources.Length);

            if (table.Columns.Columns.Any(c => !(c is IQsiAllColumnNode)))
            {
                foreach (var columnNode in table.Columns.Columns.Cast<IQsiSequentialColumnNode>())
                {
                    var column = declaredTable.NewColumn();
                    column.Name = columnNode.Alias.Name;
                }
            }
            else
            {
                using var anchorContext = new CompileContext(context);
                var anchor = await BuildTableStructure(anchorContext, source.Sources[0]);

                foreach (var anchorColumn in anchor.Columns)
                {
                    var column = declaredTable.NewColumn();
                    column.Name = anchorColumn.Name;
                }

                sourceOffset++;
                structures.Add(anchor);
            }

            for (int i = sourceOffset; i < source.Sources.Length; i++)
            {
                using var tempContext = new CompileContext(context);
                tempContext.AddDirective(declaredTable);

                structures.Add(await BuildTableStructure(tempContext, source.Sources[i]));
            }

            foreach (var structure in structures)
            {
                if (declaredTable.Columns.Count != structure.Columns.Count)
                    throw new QsiException(QsiError.DifferentColumnsCount);

                for (int i = 0; i < structure.Columns.Count; i++)
                {
                    declaredTable.Columns[i].References.Add(structure.Columns[i]);
                }
            }

            return declaredTable;
        }

        private async ValueTask BuildDirectives(CompileContext context, IQsiTableDirectivesNode directivesNode)
        {
            foreach (var directiveTable in directivesNode.Tables)
            {
                var cteName = directiveTable.Alias.Name;

                if (directivesNode.IsRecursive && ContainsRecursiveQuery(directiveTable.Source, cteName))
                {
                    if (!(directiveTable.Source is IQsiCompositeTableNode compositeTableNode))
                        throw new QsiException(QsiError.NoTopLevelUnionInRecursiveQuery, cteName);

                    if (ContainsRecursiveQuery(compositeTableNode.Sources[0], cteName))
                    {
                        if (!_options.UseAutoFixRecursiveQuery || compositeTableNode.Sources.Length < 2)
                            throw new QsiException(QsiError.NoAnchorInRecursiveQuery, cteName);

                        var fixedSources = compositeTableNode.Sources
                            .Select(s => new
                            {
                                Source = s,
                                Recursive = ContainsRecursiveQuery(s, cteName)
                            })
                            .OrderBy(x => x.Recursive ? 1 : 0)
                            .ToArray();

                        if (fixedSources[0].Recursive)
                            throw new QsiException(QsiError.NoAnchorInRecursiveQuery, cteName);

                        compositeTableNode = new CompositeTableNodeProxy(
                            compositeTableNode.Parent,
                            fixedSources
                                .Select(s => s.Source)
                                .ToArray());
                    }

                    using var directiveContext = new CompileContext(context);
                    var directiveTableStructure = await BuildRecursiveCompositeTable(context, directiveTable, compositeTableNode);
                    context.AddDirective(directiveTableStructure);
                }
                else
                {
                    using var directiveContext = new CompileContext(context);
                    var directiveTableStructure = await BuildTableStructure(directiveContext, directiveTable);
                    context.AddDirective(directiveTableStructure);
                }
            }
        }

        protected virtual bool ContainsRecursiveQuery(IQsiTableNode table, QsiIdentifier identifier)
        {
            foreach (var tableAccess in table.FindAscendants<IQsiTableAccessNode>())
            {
                if (tableAccess.Identifier.Level == 1 && Match(tableAccess.Identifier[0], identifier))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual async ValueTask<QsiDataTable> BuildJoinedTable(CompileContext context, IQsiJoinedTableNode table)
        {
            if (table.Left == null || table.Right == null)
                throw new QsiException(QsiError.Syntax);

            // priority
            // using > left > right

            var joinedTable = new QsiDataTable
            {
                Type = QsiDataTableType.Join
            };

            QsiDataTable left;
            QsiDataTable right;

            if (table.Left is IQsiJoinedTableNode leftNode)
            {
                left = await BuildJoinedTable(context, leftNode);
            }
            else
            {
                using var leftContext = new CompileContext(context);
                left = await BuildTableStructure(leftContext, table.Left);
                context.SourceTables.Add(left);
            }

            if (table.Right is IQsiJoinedTableNode rightNode)
            {
                right = await BuildJoinedTable(context, rightNode);
            }
            else
            {
                using var rightContext = new CompileContext(context);
                right = await BuildTableStructure(rightContext, table.Right);
                context.SourceTables.Add(right);
            }

            IQsiDeclaredColumnNode[] pivots = table.PivotColumns?.Columns
                .Cast<IQsiDeclaredColumnNode>()
                .ToArray();

            HashSet<QsiDataColumn> leftColumns = left.Columns.ToHashSet();
            HashSet<QsiDataColumn> rightColumns = right.Columns.ToHashSet();

            if (pivots?.Length > 0)
            {
                var pivotPairs = new PivotColumnPair[pivots.Length];

                for (int i = 0; i < pivotPairs.Length; i++)
                {
                    var pivotColumnName = pivots[i].Name[^1];
                    var leftColumnIndex = left.Columns.IndexOf(c => Match(c.Name, pivotColumnName));
                    var rightColumnIndex = right.Columns.IndexOf(c => Match(c.Name, pivotColumnName));

                    if (leftColumnIndex == -1 || rightColumnIndex == -1)
                    {
                        throw new QsiException(QsiError.UnableResolveColumn, pivots[i].Name);
                    }

                    pivotPairs[i] = new PivotColumnPair(
                        leftColumnIndex,
                        left.Columns[leftColumnIndex],
                        right.Columns[rightColumnIndex]);
                }

                foreach (var pair in pivotPairs.OrderBy(p => p.Order))
                {
                    var column = joinedTable.NewColumn();
                    column.Name = pair.Left.Name;
                    column.References.Add(pair.Left);
                    column.References.Add(pair.Right);

                    leftColumns.Remove(pair.Left);
                    rightColumns.Remove(pair.Right);
                }
            }

            foreach (var leftColumn in leftColumns)
            {
                var column = joinedTable.NewColumn();
                column.Name = leftColumn.Name;
                column.References.Add(leftColumn);
            }

            foreach (var rightColumn in rightColumns)
            {
                var column = joinedTable.NewColumn();
                column.Name = rightColumn.Name;
                column.References.Add(rightColumn);
            }

            return joinedTable;
        }

        protected virtual async ValueTask<QsiDataTable> BuildCompositeTable(CompileContext context, IQsiCompositeTableNode table)
        {
            if (table.Sources == null || table.Sources.Length == 0)
                throw new QsiException(QsiError.Syntax);

            var sources = new QsiDataTable[table.Sources.Length];

            for (int i = 0; i < sources.Length; i++)
            {
                using var tempContext = new CompileContext(context);
                sources[i] = await BuildTableStructure(tempContext, table.Sources[i]);
            }

            int columnCount = sources[0].Columns.Count;

            if (sources.Skip(1).Any(s => s.Columns.Count != columnCount))
                throw new QsiException(QsiError.DifferentColumnsCount);

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
        #endregion

        #region Column
        private IEnumerable<QsiDataColumn> ResolveColumns(CompileContext context, IQsiColumnNode column)
        {
            switch (column)
            {
                case IQsiAllColumnNode allColumn:
                    return ResolveAllColumns(context, allColumn);

                case IQsiDeclaredColumnNode declaredColumn:
                    return new[] { ResolveDeclaredColumn(context, declaredColumn) };

                case IQsiDerivedColumnNode derivedColumn:
                    return ResolveDerivedColumns(context, derivedColumn);

                case IQsiSequentialColumnNode sequentialColumn:
                    return new[] { ResolveSequentialColumn(context, sequentialColumn) };

                case IQsiBindingColumnNode bindingColumn:
                    // Process on 
                    break;
            }

            throw new InvalidOperationException();
        }

        protected virtual IEnumerable<QsiDataColumn> ResolveAllColumns(CompileContext context, IQsiAllColumnNode column)
        {
            // *
            if (column.Path == null)
            {
                if (context.SourceTable == null)
                    throw new QsiException(QsiError.NoTablesUsed);

                return column.IncludeInvisibleColumns ?
                    context.SourceTable.Columns :
                    context.SourceTable.VisibleColumns;
            }

            // path.or.alias.*

            QsiDataTable[] tables = LookupDataTablesInExpression(context, column.Path).ToArray();

            if (tables.Length == 0)
                throw new QsiException(QsiError.UnknownTable, column.Path);

            return tables.SelectMany(t => column.IncludeInvisibleColumns ? t.Columns : t.VisibleColumns);
        }

        protected virtual QsiDataColumn ResolveDeclaredColumn(CompileContext context, IQsiDeclaredColumnNode columnn)
        {
            IEnumerable<QsiDataTable> sources = Enumerable.Empty<QsiDataTable>();

            if (columnn.Name.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(columnn.Name[..^1]);
                sources = LookupDataTablesInExpression(context, identifier).ToArray();

                if (!sources.Any())
                    throw new QsiException(QsiError.UnknownTableIn, identifier, scopeFieldList);
            }
            else if (columnn.Name.Level == 0)
            {
                throw new InvalidOperationException();
            }
            else if (context.SourceTable != null)
            {
                sources = new[] { context.SourceTable };
            }

            var lastName = columnn.Name[^1];

            QsiDataColumn[] columns = sources
                .SelectMany(s => s.Columns.Where(c => Match(c.Name, lastName)))
                .Take(2)
                .ToArray();

            if (columns.Length == 0)
                throw new QsiException(QsiError.UnknownColumnIn, lastName.Value, scopeFieldList);

            if (columns.Length > 1)
                throw new QsiException(QsiError.AmbiguousColumnIn, columnn.Name, scopeFieldList);

            return columns[0];
        }

        protected virtual IEnumerable<QsiDataColumn> ResolveDerivedColumns(CompileContext context, IQsiDerivedColumnNode column)
        {
            if (column.IsExpression)
                return ResolveColumnsInExpression(context, column.Expression);

            return ResolveColumns(context, column.Column);
        }

        protected virtual QsiDataColumn ResolveSequentialColumn(CompileContext context, IQsiSequentialColumnNode column)
        {
            if (context.SourceTable == null)
                throw new QsiException(QsiError.NoTablesUsed);

            if (column.Ordinal >= context.SourceTable.Columns.Count)
                throw new QsiException(QsiError.SpecifiesMoreColumnNames);

            return context.SourceTable.VisibleColumns.ElementAt(column.Ordinal);
        }

        protected virtual IEnumerable<QsiDataColumn> ResolveColumnsInExpression(CompileContext context, IQsiExpressionNode expression)
        {
            if (expression == null)
                yield break;

            switch (expression)
            {
                case IQsiAssignExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Variable))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Value))
                        yield return c;

                    break;
                }

                case IQsiInvokeExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Member))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Parameters))
                        yield return c;

                    break;
                }

                case IQsiLiteralExpressionNode e:
                {
                    break;
                }

                case IQsiLogicalExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Left))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Right))
                        yield return c;

                    break;
                }

                case IQsiParametersExpressionNode e:
                {
                    foreach (var c in e.Expressions.SelectMany(x => ResolveColumnsInExpression(context, x)))
                        yield return c;

                    break;
                }

                case IQsiMultipleExpressionNode e:
                {
                    foreach (var c in e.Elements.SelectMany(x => ResolveColumnsInExpression(context, x)))
                        yield return c;

                    break;
                }

                case IQsiSwitchExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Value))
                        yield return c;

                    foreach (var c in e.Cases.SelectMany(c => ResolveColumnsInExpression(context, c)))
                        yield return c;

                    break;
                }

                case IQsiSwitchCaseExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Condition))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Consequent))
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
                    foreach (var c in ResolveColumnsInExpression(context, e.Expression))
                        yield return c;

                    break;
                }

                case IQsiColumnExpressionNode e:
                {
                    switch (e.Column)
                    {
                        case IQsiAllColumnNode _ when
                            e.FindDescendant<IQsiParametersExpressionNode, IQsiInvokeExpressionNode>(out _, out var i) &&
                            i.Member != null &&
                            i.Member.Identifier.Level == 1 &&
                            i.Member.Identifier[0].Value.Equals("COUNT", StringComparison.OrdinalIgnoreCase):
                            yield break;

                        case IQsiAllColumnNode allColumnNode:
                        {
                            foreach (var column in ResolveAllColumns(context, allColumnNode))
                                yield return column;

                            break;
                        }

                        case IQsiDeclaredColumnNode declaredColumnNode:
                            yield return ResolveDeclaredColumn(context, declaredColumnNode);

                            break;

                        case IQsiBindingColumnNode _:
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    break;
                }

                case IQsiArrayRankExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Array))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Rank))
                        yield return c;

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

                case IQsiMemberAccessExpressionNode _:
                {
                    // Skip unknown member access
                    break;
                }

                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion

        #region Table Lookup
        private QsiDataTable ResolveDataTable(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            return LookupDataTable(context, identifier) ?? throw new QsiException(QsiError.UnableResolveTable, identifier);
        }

        private QsiDataTable LookupDataTable(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            return
                context.Directives.FirstOrDefault(d => Match(d.Identifier, identifier)) ??
                _resolver.LookupTable(_resolver.ResolveQualifiedIdentifier(identifier));
        }

        private IEnumerable<QsiDataTable> LookupDataTablesInExpression(CompileContext context, QsiQualifiedIdentifier identifier)
        {
            var tables = new List<QsiDataTable>();

            if (context.SourceTable != null)
                tables.Add(context.SourceTable);

            tables.AddRange(context.SourceTables);

            foreach (var table in tables.Where(t => t.HasIdentifier))
            {
                // * case - Explicit access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT sakila.actor.column FROM sakila.actor             │
                // │        ▔▔▔▔▔▔^▔▔▔▔▔      ==     ▔▔▔▔▔▔^▔▔▔▔▔             │
                // │         └-> identifier(2)        └-> table.Identifier(2) │
                // └──────────────────────────────────────────────────────────┘ 

                if (Match(table.Identifier, identifier))
                    yield return table;

                // * case - 2 Level implicit access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT actor.column FROM sakila.actor                    │
                // │        ▔▔▔▔▔      <       ▔▔▔▔▔^▔▔▔▔▔                    │
                // │         └-> identifier(1)  └-> table.Identifier(2)       │
                // └──────────────────────────────────────────────────────────┘ 

                // * case - 3 Level implicit access
                // ┌──────────────────────────────────────────────────────────┐
                // │ SELECT sakila.actor.column FROM db.sakila.actor          │
                // │        ▔▔▔▔▔▔^▔▔▔▔▔       <     ▔▔^▔▔▔▔▔▔^▔▔▔▔▔          │
                // │         └-> identifier(2)        └-> table.Identifier(3) │
                // └──────────────────────────────────────────────────────────┘ 

                if (_options.UseExplicitRelationAccess)
                    continue;

                if (!IsReferenceType(table.Type))
                    continue;

                if (table.Identifier.Level <= identifier.Level)
                    continue;

                QsiIdentifier[] partialIdentifiers = table.Identifier[^identifier.Level..];
                var partialIdentifier = new QsiQualifiedIdentifier(partialIdentifiers);

                if (Match(partialIdentifier, identifier))
                    yield return table;
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
                if (!Match(a[i], b[i]))
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
        #endregion

        private class PivotColumnPair
        {
            public int Order { get; }

            public QsiDataColumn Left { get; }

            public QsiDataColumn Right { get; }

            public PivotColumnPair(int order, QsiDataColumn left, QsiDataColumn right)
            {
                Order = order;
                Left = left;
                Right = right;
            }
        }
    }
}
