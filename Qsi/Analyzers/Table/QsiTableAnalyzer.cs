using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table.Context;
using Qsi.Tree.Immutable;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Analyzers.Table
{
    public class QsiTableAnalyzer : QsiAnalyzerBase
    {
        private const string scopeFieldList = "field list";

        public QsiTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        #region Execute
        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return
                tree is IQsiTableNode &&
                (script.ScriptType == QsiScriptType.With || script.ScriptType == QsiScriptType.Select);
        }

        protected override async ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context)
        {
            if (!(context.Tree is IQsiTableNode tableNode))
                throw new InvalidOperationException();

            using var scope = new TableCompileContext(context);
            var table = await BuildTableStructure(scope, tableNode);

            return new QsiTableAnalysisResult(table);
        }
        #endregion

        #region Table
        internal async ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
        {
            context.ThrowIfCancellationRequested();

            switch (table)
            {
                case IQsiTableAccessNode tableAccess:
                    return await BuildTableAccessStructure(context, tableAccess);

                case IQsiDerivedTableNode derivedTable:
                    return await BuildDerivedTableStructure(context, derivedTable);

                case IQsiInlineDerivedTableNode inlineDerivedTableNode:
                    return await BuildInlineDerivedTableStructure(context, inlineDerivedTableNode);

                case IQsiJoinedTableNode joinedTable:
                    return await BuildJoinedTableStructure(context, joinedTable);

                case IQsiCompositeTableNode compositeTable:
                    return await BuildCompositeTableStructure(context, compositeTable);
            }

            throw new InvalidOperationException();
        }

        protected virtual async ValueTask<QsiTableStructure> BuildTableAccessStructure(TableCompileContext context, IQsiTableAccessNode table)
        {
            context.ThrowIfCancellationRequested();

            var lookup = ResolveTableStructure(context, table.Identifier);

            // view
            if (context.Options.UseViewTracing &&
                !lookup.IsSystem &&
                (lookup.Type == QsiTableType.View || lookup.Type == QsiTableType.MaterializedView))
            {
                var script = context.Engine.RepositoryProvider.LookupDefinition(lookup.Identifier, lookup.Type) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, lookup.Identifier);

                var viewTable = (IQsiTableNode)context.Engine.TreeParser.Parse(script) ??
                                throw new QsiException(QsiError.Internal, "Invalid view node");

                var typeBackup = lookup.Type;

                using var viewCompileContext = new TableCompileContext(context);

                if (lookup.Identifier.Level > 1)
                    viewCompileContext.PushIdentifierScope(lookup.Identifier.SubIdentifier(..^1));

                var viewTableStructure = await BuildTableStructure(viewCompileContext, viewTable);

                viewTableStructure.Identifier = ResolveQualifiedIdentifier(context, viewTableStructure.Identifier);
                lookup = viewTableStructure;
                lookup.Type = typeBackup;
            }

            return lookup;
        }

        protected virtual async ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
        {
            context.ThrowIfCancellationRequested();

            using var scopedContext = new TableCompileContext(context);

            // Directives

            if (table.Directives?.Tables?.Length > 0)
            {
                await BuildDirectives(scopedContext, table.Directives);
            }

            // Table Source

            var alias = table.Alias?.Name;

            if (alias == null &&
                table.Parent is IQsiDerivedColumnNode &&
                !context.Options.AllowNoAliasInDerivedTable)
            {
                throw new QsiException(QsiError.NoAlias);
            }

            if (table.Source is IQsiJoinedTableNode joinedTableNode)
            {
                scopedContext.SourceTable = await BuildJoinedTableStructure(scopedContext, joinedTableNode);
            }
            else if (table.Source != null)
            {
                using var sourceContext = new TableCompileContext(scopedContext);
                scopedContext.SourceTable = await BuildTableStructure(scopedContext, table.Source);
            }

            var declaredTable = new QsiTableStructure
            {
                Type = QsiTableType.Derived,
                Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
            };

            if (scopedContext.SourceTable != null)
                declaredTable.References.Add(scopedContext.SourceTable);

            var columns = table.Columns;

            if (columns == null || columns.Count == 0)
            {
                if (!context.Options.AllowEmptyColumnsInSelect)
                {
                    throw new QsiException(QsiError.Syntax);
                }
            }
            else if (columns.TryCast(out IQsiSequentialColumnNode[] sequentialColumns))
            {
                // Sequential columns definition

                if (scopedContext.SourceTable == null)
                    throw new QsiException(QsiError.NoTablesUsed);

                var columnType = sequentialColumns[0].ColumnType;
                QsiTableColumn[] allColumns = scopedContext.SourceTable.VisibleColumns.ToArray();
                int columnLength = allColumns.Length;

                if (sequentialColumns.Length > allColumns.Length)
                {
                    throw new QsiException(QsiError.SpecifiesMoreColumnNames);
                }

                if (columnType == QsiSequentialColumnType.Default)
                {
                    if (sequentialColumns.Length != allColumns.Length)
                        throw new QsiException(QsiError.DifferentColumnsCount);

                    columnLength = sequentialColumns.Length;
                }

                for (int i = 0; i < columnLength; i++)
                {
                    var column = allColumns[i];
                    var declaredColumn = declaredTable.NewColumn();

                    declaredColumn.Name = i < sequentialColumns.Length ? sequentialColumns[i].Alias?.Name : column.Name;
                    declaredColumn.References.Add(column);
                }
            }
            else
            {
                // Compund columns definition

                foreach (var column in columns)
                {
                    IEnumerable<QsiTableColumn> resolvedColumns = ResolveColumns(scopedContext, column);

                    switch (column)
                    {
                        case IQsiDerivedColumnNode derivedColum:
                        {
                            var declaredColumn = declaredTable.NewColumn();

                            declaredColumn.Name = derivedColum.Alias?.Name;
                            declaredColumn.IsExpression = derivedColum.IsExpression;
                            declaredColumn.References.AddRange(resolvedColumns);
                            break;
                        }

                        case IQsiBindingColumnNode bindingColumn:
                        {
                            var declaredColumn = declaredTable.NewColumn();
                            declaredColumn.Name = new QsiIdentifier(bindingColumn.Id, false);
                            declaredColumn.IsBinding = true;
                            break;
                        }

                        default:
                        {
                            foreach (var c in resolvedColumns)
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

        protected virtual ValueTask<QsiTableStructure> BuildInlineDerivedTableStructure(TableCompileContext context, IQsiInlineDerivedTableNode table)
        {
            context.ThrowIfCancellationRequested();

            var alias = table.Alias?.Name;

            if (alias == null &&
                table.Parent is IQsiDerivedColumnNode &&
                !context.Options.AllowNoAliasInDerivedTable)
            {
                throw new QsiException(QsiError.NoAlias);
            }

            var declaredTable = new QsiTableStructure
            {
                Type = QsiTableType.Inline,
                Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
            };

            int? columnCount = null;

            switch (table.Columns)
            {
                case null:
                case var cd when cd.All(c => c is IQsiAllColumnNode all && all.Path == null):
                    // Skip
                    break;

                case var cd when cd.TryCast(out IQsiSequentialColumnNode[] sequentialColumns):
                    foreach (var column in sequentialColumns)
                    {
                        var c = declaredTable.NewColumn();
                        c.Name = column.Alias.Name;
                    }

                    columnCount = sequentialColumns.Length;
                    break;

                default:
                    throw new NotSupportedException("Not supported columns in inline derived table.");
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

            if ((columnCount ?? 0) == 0)
            {
                if (!context.Options.AllowEmptyColumnsInInline)
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

            return new ValueTask<QsiTableStructure>(declaredTable);
        }

        protected virtual async ValueTask<QsiTableStructure> BuildRecursiveCompositeTableStructure(TableCompileContext context, IQsiDerivedTableNode table, IQsiCompositeTableNode source)
        {
            context.ThrowIfCancellationRequested();

            var declaredTable = new QsiTableStructure
            {
                Type = QsiTableType.Derived,
                Identifier = new QsiQualifiedIdentifier(table.Alias.Name)
            };

            int sourceOffset = 0;
            var structures = new List<QsiTableStructure>(source.Sources.Length);

            if (table.Columns.Any(c => !(c is IQsiAllColumnNode)))
            {
                foreach (var columnNode in table.Columns.Cast<IQsiSequentialColumnNode>())
                {
                    var column = declaredTable.NewColumn();
                    column.Name = columnNode.Alias.Name;
                }
            }
            else
            {
                using var anchorContext = new TableCompileContext(context);
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
                using var tempContext = new TableCompileContext(context);
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

        internal async ValueTask BuildDirectives(TableCompileContext context, IQsiTableDirectivesNode directivesNode)
        {
            context.ThrowIfCancellationRequested();

            foreach (var directiveTable in directivesNode.Tables)
            {
                var cteName = directiveTable.Alias.Name;

                if (directivesNode.IsRecursive && ContainsRecursiveQuery(directiveTable.Source, cteName))
                {
                    if (!(directiveTable.Source is IQsiCompositeTableNode compositeTableNode))
                        throw new QsiException(QsiError.NoTopLevelUnionInRecursiveQuery, cteName);

                    if (ContainsRecursiveQuery(compositeTableNode.Sources[0], cteName))
                    {
                        if (!context.Options.UseAutoFixRecursiveQuery || compositeTableNode.Sources.Length < 2)
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

                        compositeTableNode = new ImmutableCompositeTableNode(
                            compositeTableNode.Parent,
                            fixedSources
                                .Select(s => s.Source)
                                .ToArray(),
                            compositeTableNode.OrderExpression,
                            compositeTableNode.LimitExpression,
                            compositeTableNode.UserData);
                    }

                    using var directiveContext = new TableCompileContext(context);
                    var directiveTableStructure = await BuildRecursiveCompositeTableStructure(context, directiveTable, compositeTableNode);
                    context.AddDirective(directiveTableStructure);
                }
                else
                {
                    using var directiveContext = new TableCompileContext(context);
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

        protected virtual async ValueTask<QsiTableStructure> BuildJoinedTableStructure(TableCompileContext context, IQsiJoinedTableNode table)
        {
            context.ThrowIfCancellationRequested();

            if (table.Left == null || table.Right == null)
                throw new QsiException(QsiError.Syntax);

            // priority
            // using > left > right

            var joinedTable = new QsiTableStructure
            {
                Type = QsiTableType.Join
            };

            QsiTableStructure left;
            QsiTableStructure right;

            if (table.Left is IQsiJoinedTableNode leftNode)
            {
                left = await BuildJoinedTableStructure(context, leftNode);
            }
            else
            {
                using var leftContext = new TableCompileContext(context);
                left = await BuildTableStructure(leftContext, table.Left);
                context.SourceTables.Add(left);
            }

            if (table.Right is IQsiJoinedTableNode rightNode)
            {
                right = await BuildJoinedTableStructure(context, rightNode);
            }
            else
            {
                using var rightContext = new TableCompileContext(context);
                right = await BuildTableStructure(rightContext, table.Right);
                context.SourceTables.Add(right);
            }

            joinedTable.References.Add(left);
            joinedTable.References.Add(right);

            IQsiDeclaredColumnNode[] pivots = table.PivotColumns?
                .Cast<IQsiDeclaredColumnNode>()
                .ToArray();

            HashSet<QsiTableColumn> leftColumns = left.Columns.ToHashSet();
            HashSet<QsiTableColumn> rightColumns = right.Columns.ToHashSet();

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

        protected virtual async ValueTask<QsiTableStructure> BuildCompositeTableStructure(TableCompileContext context, IQsiCompositeTableNode table)
        {
            context.ThrowIfCancellationRequested();

            if (table.Sources == null || table.Sources.Length == 0)
                throw new QsiException(QsiError.Syntax);

            var sources = new QsiTableStructure[table.Sources.Length];

            for (int i = 0; i < sources.Length; i++)
            {
                using var tempContext = new TableCompileContext(context);
                sources[i] = await BuildTableStructure(tempContext, table.Sources[i]);
            }

            int columnCount = sources[0].Columns.Count;

            if (sources.Skip(1).Any(s => s.Columns.Count != columnCount))
                throw new QsiException(QsiError.DifferentColumnsCount);

            var compositeSource = new QsiTableStructure
            {
                Type = QsiTableType.Union
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
        private IEnumerable<QsiTableColumn> ResolveColumns(TableCompileContext context, IQsiColumnNode column)
        {
            context.ThrowIfCancellationRequested();

            switch (column)
            {
                case IQsiAllColumnNode allColumn:
                    return ResolveAllColumns(context, allColumn);

                case IQsiDeclaredColumnNode declaredColumn:
                    return new[] { ResolveDeclaredColumn(context, declaredColumn) };

                case IQsiDerivedColumnNode derivedColumn:
                    return ResolveDerivedColumns(context, derivedColumn);

                case IQsiBindingColumnNode _:
                    return Array.Empty<QsiTableColumn>();

                case IQsiSequentialColumnNode _:
                    throw new QsiException(QsiError.SyntaxError, "You cannot define sequential column in a compound column definition.");
            }

            throw new InvalidOperationException();
        }

        protected virtual IEnumerable<QsiTableColumn> ResolveAllColumns(TableCompileContext context, IQsiAllColumnNode column)
        {
            context.ThrowIfCancellationRequested();

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

            QsiTableStructure[] tables = LookupDataTableStructuresInExpression(context, column.Path).ToArray();

            if (tables.Length == 0)
                throw new QsiException(QsiError.UnknownTable, column.Path);

            return tables.SelectMany(t => column.IncludeInvisibleColumns ? t.Columns : t.VisibleColumns);
        }

        protected virtual QsiTableColumn ResolveDeclaredColumn(TableCompileContext context, IQsiDeclaredColumnNode column)
        {
            context.ThrowIfCancellationRequested();

            IEnumerable<QsiTableStructure> sources = Enumerable.Empty<QsiTableStructure>();

            if (column.Name.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(column.Name[..^1]);
                sources = LookupDataTableStructuresInExpression(context, identifier).ToArray();

                if (!sources.Any())
                    throw new QsiException(QsiError.UnknownTableIn, identifier, scopeFieldList);
            }
            else if (column.Name.Level == 0)
            {
                throw new InvalidOperationException();
            }
            else if (context.SourceTable != null)
            {
                sources = new[] { context.SourceTable };
            }

            var lastName = column.Name[^1];

            QsiTableColumn[] columns = sources
                .SelectMany(s => s.Columns.Where(c => Match(c.Name, lastName)))
                .Take(2)
                .ToArray();

            if (columns.Length == 0)
                throw new QsiException(QsiError.UnknownColumnIn, lastName.Value, scopeFieldList);

            if (columns.Length > 1)
                throw new QsiException(QsiError.AmbiguousColumnIn, column.Name, scopeFieldList);

            return columns[0];
        }

        protected virtual IEnumerable<QsiTableColumn> ResolveDerivedColumns(TableCompileContext context, IQsiDerivedColumnNode column)
        {
            context.ThrowIfCancellationRequested();

            if (column.IsExpression)
                return ResolveColumnsInExpression(context, column.Expression);

            return ResolveColumns(context, column.Column);
        }

        protected virtual IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            context.ThrowIfCancellationRequested();

            if (expression == null)
                yield break;

            switch (expression)
            {
                case IQsiSetColumnExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Value))
                        yield return c;

                    break;
                }

                case IQsiSetVariableExpressionNode e:
                {
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
                    using var scopedContext = new TableCompileContext(context);
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

                case IQsiMemberAccessExpressionNode e:
                {
                    foreach (var c in ResolveColumnsInExpression(context, e.Target))
                        yield return c;

                    foreach (var c in ResolveColumnsInExpression(context, e.Member))
                        yield return c;

                    break;
                }

                case IQsiVariableExpressionNode e:
                {
                    // TODO: Analyze variable
                    break;
                }

                case IQsiFunctionExpressionNode e:
                {
                    // TODO: Analyze function
                    break;
                }

                case IQsiMemberExpressionNode _:
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
        private QsiQualifiedIdentifier ResolveQualifiedIdentifier(TableCompileContext context, QsiQualifiedIdentifier identifier)
        {
            context.ThrowIfCancellationRequested();

            // Qualified identifier without table
            var identifierScope = context.PeekIdentifierScope();

            if (identifierScope != null && identifier.Level <= identifierScope.Level)
            {
                int offset = identifierScope.Level - identifier.Level + 1;
                Span<QsiIdentifier> identifiers = new QsiIdentifier[identifierScope.Level + 1].AsSpan();

                identifierScope._identifiers[..offset].CopyTo(identifiers);
                identifier._identifiers.AsSpan().CopyTo(identifiers.Slice(offset));

                identifier = new QsiQualifiedIdentifier(identifiers.ToArray());
            }

            return context.Engine.RepositoryProvider.ResolveQualifiedIdentifier(identifier);
        }

        private QsiTableStructure ResolveTableStructure(TableCompileContext context, QsiQualifiedIdentifier identifier)
        {
            context.ThrowIfCancellationRequested();
            return LookupTableStructure(context, identifier) ?? throw new QsiException(QsiError.UnableResolveTable, identifier);
        }

        private QsiTableStructure LookupTableStructure(TableCompileContext context, QsiQualifiedIdentifier identifier)
        {
            context.ThrowIfCancellationRequested();

            return
                context.Directives.FirstOrDefault(d => Match(d.Identifier, identifier)) ??
                context.Engine.RepositoryProvider.LookupTable(ResolveQualifiedIdentifier(context, identifier));
        }

        private IEnumerable<QsiTableStructure> LookupDataTableStructuresInExpression(TableCompileContext context, QsiQualifiedIdentifier identifier)
        {
            context.ThrowIfCancellationRequested();

            var tables = new List<QsiTableStructure>();

            if (context.SourceTable != null)
                tables.Add(context.SourceTable);

            tables.AddRange(context.SourceTables);

            return tables.Where(t => Match(context, t, identifier));
        }
        #endregion

        private class PivotColumnPair
        {
            public int Order { get; }

            public QsiTableColumn Left { get; }

            public QsiTableColumn Right { get; }

            public PivotColumnPair(int order, QsiTableColumn left, QsiTableColumn right)
            {
                Order = order;
                Left = left;
                Right = right;
            }
        }
    }
}
