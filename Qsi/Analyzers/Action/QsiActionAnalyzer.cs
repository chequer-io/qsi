using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qsi.Analyzers.Action.Context;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Data;
using Qsi.Tree.Immutable;
using Qsi.Utilities;

namespace Qsi.Analyzers.Action
{
    public class QsiActionAnalyzer : QsiAnalyzerBase
    {
        protected delegate QsiDataValue DataValueSelector(DataManipulationTargetColumnPivot pivot);

        public QsiActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return tree is IQsiActionNode;
        }

        protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
        {
            switch (context.Tree)
            {
                case IQsiPrepareActionNode prepareActionNode:
                    return await ExecutePrepareAction(context, prepareActionNode);

                case IQsiDropPrepareActionNode dropPrepareAction:
                    return await ExecuteDropPrepareAction(context, dropPrepareAction);

                case IQsiExecutePrepareActionNode executeAction:
                    return await ExecuteExecutePrepareAction(context, executeAction);

                case IQsiDataInsertActionNode dataInsertAction:
                    return await ExecuteDataInsertAction(context, dataInsertAction);

                case IQsiDataDeleteActionNode dataDeleteAction:
                    return await ExecuteDataDeleteAction(context, dataDeleteAction);

                case IQsiDataUpdateActionNode dataUpdateAction:
                    return await ExecuteDataUpdateAction(context, dataUpdateAction);

                case IQsiChangeSearchPathActionNode searchPathAction:
                    return await ExecuteSearchPathAction(context, searchPathAction);

                default:
                    throw TreeHelper.NotSupportedTree(context.Tree);
            }
        }

        #region Prepared
        protected virtual ValueTask<IQsiAnalysisResult[]> ExecutePrepareAction(IAnalyzerContext context, IQsiPrepareActionNode action)
        {
            string query;

            switch (action.Query)
            {
                case IQsiLiteralExpressionNode { Type: QsiDataType.String } literal:
                    query = literal.Value?.ToString();
                    break;

                case IQsiVariableExpressionNode variableAccess:
                    var variable = context.Engine.RepositoryProvider.LookupVariable(variableAccess.Identifier) ??
                                   throw new QsiException(QsiError.UnknownVariable, variableAccess.Identifier);

                    if (variable.Type != QsiDataType.String)
                        throw new InvalidOperationException();

                    query = variable.Value.ToString();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var scriptType = context.Engine.ScriptParser.GetSuitableType(query);

            var refAction = new QsiReferenceAction
            {
                Type = QsiReferenceType.Prepared,
                Operation = QsiReferenceOperation.Create,
                IsolationLevel = QsiReferenceIsolationLevel.Session,
                Target = action.Identifier,
                Definition = new QsiScript(query, scriptType)
            };

            return refAction.ToSingleArray().AsValueTask();
        }

        protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteDropPrepareAction(IAnalyzerContext context, IQsiDropPrepareActionNode action)
        {
            var refAction = new QsiReferenceAction
            {
                Type = QsiReferenceType.Prepared,
                Operation = QsiReferenceOperation.Delete,
                IsolationLevel = QsiReferenceIsolationLevel.Session,
                Target = action.Identifier
            };

            return refAction.ToSingleArray().AsValueTask();
        }

        protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteExecutePrepareAction(IAnalyzerContext context, IQsiExecutePrepareActionNode action)
        {
            var definition = context.Engine.RepositoryProvider.LookupDefinition(action.Identifier, QsiTableType.Prepared) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, action.Identifier);

            return context.Engine.Execute(definition, null);
        }
        #endregion

        #region Table Data
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiDataValue ResolveDefaultColumnValue(DataManipulationTargetColumnPivot pivot)
        {
            if (pivot.TargetColumn.Default != null)
                return new QsiDataValue(pivot.TargetColumn.Default, QsiDataType.Raw);

            return QsiDataValue.Default;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected QsiDataValue ResolveColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
        {
            if (expression is IQsiLiteralExpressionNode literal)
                return new QsiDataValue(literal.Value, literal.Type);

            return ResolveRawColumnValue(context, expression);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiDataValue ResolveRawColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
        {
            IQsiTableExpressionNode[] subqueries = CollectSubqueries(expression).ToArray();

            if (subqueries.Length == 0)
            {
                var value = context.Engine.TreeDeparser.Deparse(expression, context.Script);
                return new QsiDataValue(value, QsiDataType.Raw);
            }

            // ex) PostgreSql
            if (expression.UserData?.GetData(QsiNodeProperties.Span) is not { } span)
                throw TreeHelper.NotSupportedFeature("subqueries value");

            var subqueriesWithSpan = subqueries
                .Select(x => new { Node = x, Span = x.UserData?.GetData(QsiNodeProperties.Span) })
                .OrderBy(x => x.Span is { } s ? s.Start.GetOffset(context.Script.Script.Length) : -1)
                .ToArray();

            var builder = new StringBuilder(context.Script.Script[span]);
            var baseOffset = span.Start.GetOffset(context.Script.Script.Length);
            var offset = 0;

            foreach (var x in subqueriesWithSpan)
            {
                if (x.Span is null)
                    continue;

                var (sOffset, sLength) = x.Span.Value.GetOffsetAndLength(context.Script.Script.Length);
                var index = sOffset - baseOffset + offset;
                string computedValue = ComputeSubqueryValue(context, x.Node);

                builder.Remove(index, sLength);
                builder.Insert(index, computedValue);

                offset += computedValue.Length - sLength;
            }

            return new QsiDataValue(builder.ToString(), QsiDataType.Raw);

            IEnumerable<IQsiTableExpressionNode> CollectSubqueries(IQsiExpressionNode node)
            {
                var queue = new Queue<IQsiTreeNode>();
                queue.Enqueue(node);

                while (queue.TryDequeue(out var item))
                {
                    if (item is IQsiTableExpressionNode tNode)
                    {
                        yield return tNode;
                    }
                    else
                    {
                        foreach (var child in item.Children ?? Enumerable.Empty<IQsiTreeNode>())
                            queue.Enqueue(child);
                    }
                }
            }
        }

        private string ComputeSubqueryValue(IAnalyzerContext context, IQsiTableExpressionNode node)
        {
            var query = context.Engine.TreeDeparser.Deparse(node.Table, context.Script);
            var scriptType = context.Engine.ScriptParser.GetSuitableType(query);
            var script = new QsiScript(query, scriptType);
            QsiParameter[] parameters = ArrangeBindParameters(context, node.Table);

            using var reader = context.Engine.RepositoryProvider.GetDataReaderAsync(script, parameters, default).Result;

            if (!reader.Read())
                return "null";

            var builder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0)
                    builder.Append(',');

                builder.Append(FormatValue(reader.GetValue(i)));
            }

            if (reader.Read())
                throw new QsiException(QsiError.SubqueryReturnsMoreThanRow, 1);

            // 1, 2 -> (1, 2)
            if (reader.FieldCount > 1)
            {
                builder.Insert(0, '(');
                builder.Append(')');
            }

            return builder.ToString();

            static string FormatValue(object value)
            {
                return value switch
                {
                    null => "null",
                    string str => str,
                    _ => value.ToString()
                };
            }
        }

        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(IAnalyzerContext context, QsiTableStructure table)
        {
            return ResolveDataManipulationTargets(context, ResolveColumnTargetsFromTable(context, table));
        }

        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(IAnalyzerContext context, IEnumerable<ColumnTarget> columnTargets)
        {
            IEnumerable<DataManipulationTargetColumnPivot> rawPivots = columnTargets
                .Select(ResolveDataManipulationTargetColumn);

            return ResolveDataManipulationTargetsCore(context, rawPivots);
        }

        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargetsCore(IAnalyzerContext context, IEnumerable<DataManipulationTargetColumnPivot> rawPivots)
        {
            return rawPivots
                .GroupBy(c => c.TargetColumn.Parent)
                .Select(g =>
                {
                    var buffer = new DataManipulationTargetColumnPivot[g.Key.Columns.Count];

                    foreach (var pivot in g)
                        buffer[pivot.TargetOrder] = pivot;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (buffer[i] != default)
                            continue;

                        buffer[i] = new DataManipulationTargetColumnPivot(i, g.Key.Columns[i], -1, null);
                    }

                    return new DataManipulationTarget(g.Key, buffer, context.Engine.CacheProviderFactory);
                });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual DataManipulationTargetColumnPivot ResolveDataManipulationTargetColumn(ColumnTarget columnTarget)
        {
            if (columnTarget.TargetColumn.IsAnonymous)
                throw new QsiException(QsiError.NotUpdatableColumn, "anonymous");

            if (columnTarget.TargetColumn.IsBinding || columnTarget.TargetColumn.IsExpression || !columnTarget.TargetColumn.IsVisible)
                throw new QsiException(QsiError.NotUpdatableColumn, columnTarget.TargetColumn.Name);

            return new DataManipulationTargetColumnPivot(
                columnTarget.AffectedReferenceColumn.Parent.Columns.IndexOf(columnTarget.AffectedReferenceColumn),
                columnTarget.AffectedReferenceColumn,
                columnTarget.DeclaredOrder,
                columnTarget.TargetColumn
            );
        }

        protected virtual QsiTableColumn[] GetAffectedColumns(DataManipulationTarget target)
        {
            return target.ColumnPivots
                .Where(p => p.DeclaredColumn != null)
                .Select(p => p.TargetColumn)
                .ToArray();
        }

        protected IEnumerable<QsiTableColumn> GetAffectedColumns(QsiTableColumn column)
        {
            return QsiUtility.FlattenColumns(column)
                .Where(x =>
                {
                    if (x.Parent is not { } parent)
                        return false;

                    return parent.Type == QsiTableType.Table;
                });
        }

        protected virtual IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            switch (node)
            {
                case IQsiDerivedTableNode derivedTable:
                    return new ImmutableDerivedTableNode(
                        derivedTable.Parent,
                        derivedTable.Directives,
                        derivedTable.Columns,
                        derivedTable.Source,
                        null,
                        derivedTable.Where,
                        derivedTable.Grouping,
                        derivedTable.Order,
                        derivedTable.Limit,
                        null);

                case IQsiCompositeTableNode compositeTable:
                    return compositeTable.ToImmutable(true);

                case IQsiTableFunctionNode tableFunction:
                    return tableFunction.ToImmutable(true);

                default:
                    return new ImmutableDerivedTableNode(
                        node.Parent,
                        null,
                        TreeHelper.CreateAllColumnsDeclaration(),
                        node,
                        null, null, null, null, null, null);
            }
        }

        protected virtual QsiParameter[] ArrangeBindParameters(IAnalyzerContext context, IQsiTreeNode node)
        {
            IQsiBindParameterExpressionNode[] parameterNodes = node
                .FindAscendants<IQsiBindParameterExpressionNode>()
                .ToArray();

            var result = new QsiParameter[parameterNodes.Length];
            var index = 0;

            foreach (var parameterNode in parameterNodes)
            {
                if (!context.Parameters.TryGetValue(parameterNode, out var parameter))
                {
                    var parameterName = context.Engine.TreeDeparser.Deparse(parameterNode, context.Script);
                    throw new QsiException(QsiError.ParameterNotFound, parameterName);
                }

                // TODO: index bind parameter arrangement not implemented
                if (parameterNode.Type == QsiParameterType.Index)
                    return context.Parameters.Values.ToArray();

                result[index++] = parameter;
            }

            return result;
        }

        protected virtual async ValueTask<QsiDataTable> GetDataTableByCommonTableNode(IAnalyzerContext context, IQsiTableNode commonTableNode)
        {
            var query = context.Engine.TreeDeparser.Deparse(commonTableNode, context.Script);
            var scriptType = context.Engine.ScriptParser.GetSuitableType(query);
            var script = new QsiScript(query, scriptType);
            QsiParameter[] parameters = ArrangeBindParameters(context, commonTableNode);

            return await context.Engine.RepositoryProvider.GetDataTable(script, parameters, context.CancellationToken);
        }
        #endregion

        #region Insert, Replace
        protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataInsertAction(IAnalyzerContext context, IQsiDataInsertActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new TableCompileContext(context))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);
            }

            ColumnTarget[] columnTargets = ResolveColumnTargetsFromDataInsertAction(context, table, action);

            var dataContext = new TableDataInsertContext(context, table)
            {
                ColumnTargets = columnTargets,
                Targets = ResolveDataManipulationTargets(context, columnTargets).ToArray()
            };

            if (action.ValueTable != null)
            {
                await ProcessQueryValues(dataContext, action.Directives, action.ValueTable);
            }
            else if (!ListUtility.IsNullOrEmpty(action.Values))
            {
                ProcessValues(dataContext, action.Values);
            }
            else if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                ProcessSetValues(dataContext, action.SetValues);
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            if (action.ConflictAction != null && action.ConflictBehavior != QsiDataConflictBehavior.Ignore)
            {
                ProcessConflict(dataContext, action.ConflictAction, action.ConflictBehavior);
            }

            return dataContext.Targets
                .Select(t => new QsiDataManipulationResult
                {
                    Table = t.Table,
                    AffectedColumns = GetAffectedColumns(t),
                    InsertRows = t.InsertRows.ToNullIfEmpty(),
                    DuplicateRows = t.DuplicateRows.ToNullIfEmpty()
                })
                .ToArray<IQsiAnalysisResult>();
        }

        protected virtual ColumnTarget[] ResolveColumnTargetsFromDataInsertAction(IAnalyzerContext context, QsiTableStructure table, IQsiDataInsertActionNode action)
        {
            if (!ListUtility.IsNullOrEmpty(action.Columns))
                return ResolveColumnTargetsFromIdentifiers(context, table, action.Columns);

            if (!ListUtility.IsNullOrEmpty(action.SetValues))
                return ResolveSetColumnTargets(context, table, action.SetValues).ToArray<ColumnTarget>();

            return ResolveColumnTargetsFromTable(context, table);
        }

        protected virtual ColumnTarget[] ResolveColumnTargetsFromIdentifiers(IAnalyzerContext context, QsiTableStructure table, IEnumerable<QsiQualifiedIdentifier> identifiers)
        {
            return identifiers
                .Select((x, i) =>
                {
                    var index = FindColumnIndex(context, table, x);

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, x);

                    var targetColumn = table.Columns[index];

                    QsiTableColumn[] affectedColumns = GetAffectedColumns(targetColumn)
                        .Take(2)
                        .ToArray();

                    // TODO: Add QsiAnalyzerOptions
                    if (affectedColumns.Length != 1)
                        throw new QsiException(QsiError.NotUpdatableColumn, x);

                    return new ColumnTarget(i, x, targetColumn, affectedColumns[0]);
                })
                .ToArray();
        }

        protected virtual ColumnTarget[] ResolveColumnTargetsFromTable(IAnalyzerContext context, QsiTableStructure table)
        {
            return table.Columns
                .Select((x, i) =>
                {
                    QsiTableColumn[] affectedColumns = GetAffectedColumns(x)
                        .Take(2)
                        .ToArray();

                    // TODO: Add QsiAnalyzerOptions
                    if (affectedColumns.Length != 1)
                        throw new QsiException(QsiError.NotUpdatableColumn, x.Name);

                    return new ColumnTarget(i, new QsiQualifiedIdentifier(x.Name), x, affectedColumns[0]);
                })
                .ToArray();
        }

        protected virtual SetColumnTarget[] ResolveSetColumnTargets(IAnalyzerContext context, QsiTableStructure table, IEnumerable<IQsiSetColumnExpressionNode> declaredNodes)
        {
            SetColumnTarget[] pivots = declaredNodes
                .Select((x, i) => ResolveSetColumnTarget(context, table, i, x))
                .ToArray();

            IEnumerable<int> multipleDeclaration = pivots
                .GroupBy(x => x.AffectedReferenceColumn)
                .Select(x => x.Count());

            if (multipleDeclaration.Any(x => x > 1))
                throw new QsiException(QsiError.NotSupportedFeature, "Multiple set column");

            return pivots;
        }

        protected virtual SetColumnTarget ResolveSetColumnTarget(
            IAnalyzerContext context,
            QsiTableStructure table,
            int declaredOrder,
            IQsiSetColumnExpressionNode declaredNode)
        {
            var index = FindColumnIndex(context, table, declaredNode.Target);

            if (index == -1)
                throw new QsiException(QsiError.UnknownColumn, declaredNode.Target);

            var targetColumn = table.Columns[index];

            QsiTableColumn[] affectedColumns = GetAffectedColumns(targetColumn)
                .Take(2)
                .ToArray();

            // TODO: Add QsiAnalyzerOptions
            if (affectedColumns.Length != 1)
                throw new QsiException(QsiError.NotUpdatableColumn, declaredNode.Target);

            return new SetColumnTarget(
                declaredOrder,
                declaredNode.Target,
                targetColumn,
                affectedColumns[0],
                declaredNode.Value
            );
        }

        private async ValueTask ProcessQueryValues(TableDataInsertContext context, IQsiTableDirectivesNode directives, IQsiTableNode valueTable)
        {
            var engine = context.Engine;
            var script = engine.TreeDeparser.Deparse(valueTable, context.Script);

            // TODO: bind parameters in directives
            if (directives != null)
                script = $"{engine.TreeDeparser.Deparse(directives, context.Script)}\n{script}";

            var scriptType = engine.ScriptParser.GetSuitableType(script);
            QsiParameter[] parameters = ArrangeBindParameters(context, valueTable);
            var dataTable = await engine.RepositoryProvider.GetDataTable(new QsiScript(script, scriptType), parameters, context.CancellationToken);

            if (dataTable.Rows.ColumnCount != context.ColumnTargets.Length)
                throw new QsiException(QsiError.DifferentColumnsCount);

            foreach (var row in dataTable.Rows)
            {
                PopulateInsertRow(context, pivot => row.Items[pivot.DeclaredOrder]);
            }
        }

        private void ProcessValues(TableDataInsertContext context, IQsiRowValueExpressionNode[] values)
        {
            int columnCount = context.ColumnTargets.Length;

            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];

                if (columnCount != value.ColumnValues.Length)
                    throw new QsiException(QsiError.DifferentColumnValueCount, i + 1);

                PopulateInsertRow(context, pivot => ResolveColumnValue(context, value.ColumnValues[pivot.DeclaredOrder]));
            }
        }

        private void ProcessSetValues(TableDataInsertContext context, IQsiSetColumnExpressionNode[] setValues)
        {
            PopulateInsertRow(context, pivot =>
            {
                var setValue = setValues[pivot.DeclaredOrder];
                return ResolveColumnValue(context, setValue.Value);
            });
        }

        // TODO: action.Target, action.Condition
        private void ProcessConflict(TableDataInsertContext context, IQsiDataConflictActionNode action, QsiDataConflictBehavior behavior)
        {
            if (behavior == QsiDataConflictBehavior.None)
            {
                // TODO: throw in duplicate rows
                return;
            }

            if (action.SetValues == null)
                throw new QsiException(QsiError.Syntax);

            SetColumnTarget[] setColumnTargets = ResolveSetColumnTargets(context, context.Table, action.SetValues);

            foreach (var updateTarget in ResolveDataManipulationTargets(context, setColumnTargets))
            {
                var target = context.Targets.FirstOrDefault(t => t.Table == updateTarget.Table);

                if (target == null)
                    continue;

                var targetRow = new QsiDataRow(target.DuplicateRows.ColumnCount);

                foreach (var pivot in updateTarget.ColumnPivots)
                {
                    if (pivot.DeclaredColumn != null)
                    {
                        var setValue = action.SetValues[pivot.DeclaredOrder];
                        targetRow.Items[pivot.TargetOrder] = ResolveColumnValue(context, setValue.Value);
                    }
                    else
                    {
                        targetRow.Items[pivot.TargetOrder] = QsiDataValue.Unset;
                    }
                }

                target.DuplicateRows.Add(targetRow);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateInsertRow(TableDataInsertContext context, DataValueSelector valueSelector)
        {
            foreach (var target in context.Targets)
            {
                var targetRow = new QsiDataRow(target.InsertRows.ColumnCount);

                foreach (var pivot in target.ColumnPivots)
                {
                    if (pivot.DeclaredColumn != null)
                    {
                        targetRow.Items[pivot.TargetOrder] = valueSelector(pivot);
                    }
                    else
                    {
                        targetRow.Items[pivot.TargetOrder] = ResolveDefaultColumnValue(pivot);
                    }
                }

                target.InsertRows.Add(targetRow);
            }
        }
        #endregion

        #region Delete
        protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;
            int tableColumnCount;

            var actionTarget = action.Target;

            var options = context.Options with
            {
                UseImplicitTableWildcardInSelect = actionTarget is IQsiDerivedTableNode
            };

            using (var tableContext = new TableCompileContext(context, options))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, actionTarget);
                tableColumnCount = table.Columns.Count;

                // TODO: How to compile without IsImplicitTableWildcard options
                HashSet<QsiQualifiedIdentifier> implicitTableWildcardSources = table.Columns
                    .Select(x => x.ImplicitTableWildcardTarget)
                    .Where(x => x is not null)
                    .ToHashSet(QualifiedIdentifierComparer);

                if (implicitTableWildcardSources.Count > 0)
                {
                    var derived = (IQsiDerivedTableNode)actionTarget;
                    IQsiColumnNode[] columnNodes = derived.Columns.Columns.ToArray();

                    for (int i = 0; i < columnNodes.Length; i++)
                    {
                        if (columnNodes[i] is IQsiColumnReferenceNode referenceNode &&
                            implicitTableWildcardSources.Contains(referenceNode.Name))
                        {
                            columnNodes[i] = new QsiAllColumnNode { Path = referenceNode.Name };
                        }
                    }

                    actionTarget = new ImmutableDerivedTableNode(
                        derived.Parent,
                        derived.Directives,
                        new ImmutableColumnsDeclarationNode(
                            derived.Columns.Parent,
                            columnNodes,
                            null
                        ),
                        derived.Source,
                        null,
                        derived.Where,
                        derived.Grouping,
                        derived.Order,
                        derived.Limit,
                        null
                    );
                }
            }

            var commonTableNode = ReassembleCommonTableNode(actionTarget);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            if (ListUtility.IsNullOrEmpty(action.Columns) && dataTable.Table.Columns.Count != tableColumnCount)
                throw new QsiException(QsiError.Internal, "Query results do not match target table structure");

            QsiQualifiedIdentifier[] columnNames = action.Columns;

            if (columnNames == null)
            {
                columnNames = new QsiQualifiedIdentifier[tableColumnCount];

                for (int i = 0; i < tableColumnCount; i++)
                {
                    columnNames[i] = new QsiQualifiedIdentifier(dataTable.Table.Columns[i].Name);
                    table.Columns[i].Name = columnNames[i][^1];
                }
            }

            ColumnTarget[] columnTargets = ResolveColumnTargetsFromIdentifiers(context, table, columnNames);

            return ResolveDataManipulationTargets(context, columnTargets)
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        var targetRow = new QsiDataRow(target.DeleteRows.ColumnCount);

                        foreach (var pivot in target.ColumnPivots)
                        {
                            if (pivot.DeclaredColumn != null)
                            {
                                targetRow.Items[pivot.TargetOrder] = row.Items[pivot.DeclaredOrder];
                            }
                            else
                            {
                                targetRow.Items[pivot.TargetOrder] = QsiDataValue.Unset;
                            }
                        }

                        target.DeleteRows.Add(targetRow);
                    }

                    return new QsiDataManipulationResult
                    {
                        Table = target.Table,
                        AffectedColumns = GetAffectedColumns(target),
                        DeleteRows = target.DeleteRows.ToNullIfEmpty()
                    };
                })
                .ToArray<IQsiAnalysisResult>();
        }
        #endregion

        #region Update
        protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
        {
            // table structure
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);

            var sourceTable = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);

            // update data (rows)
            var commonTableNode = ReassembleCommonTableNode(action.Target);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            if (dataTable.Table.Columns.Count != sourceTable.Columns.Count)
                throw new QsiException(QsiError.DifferentColumnsCount);

            var values = new QsiDataValue[sourceTable.Columns.Count];
            var affectedColumnMap = new bool[sourceTable.Columns.Count];
            ColumnTarget[] columnTargets;

            if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                SetColumnTarget[] setColumnTargets = ResolveSetColumnTargets(context, sourceTable, action.SetValues);
                columnTargets = setColumnTargets.ToArray<ColumnTarget>();

                foreach (var columnTarget in setColumnTargets)
                {
                    var targetIndex = columnTarget.TargetColumn.Parent.Columns.IndexOf(columnTarget.TargetColumn);
                    values[targetIndex] = ResolveColumnValue(context, columnTarget.ValueNode);
                    affectedColumnMap[targetIndex] = true;
                }
            }
            else if (action.Value != null)
            {
                columnTargets = ResolveColumnTargetsFromTable(context, sourceTable);

                if (action.Value.ColumnValues.Length != values.Length)
                    throw new QsiException(QsiError.DifferentColumnValueCount, 0);

                for (int i = 0; i < values.Length; i++)
                    values[i] = ResolveColumnValue(context, action.Value.ColumnValues[i]);

                affectedColumnMap.AsSpan().Fill(true);
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            return ResolveDataManipulationTargets(context, columnTargets)
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        var oldRow = new QsiDataRow(target.UpdateBeforeRows.ColumnCount);
                        var newRow = new QsiDataRow(target.UpdateAfterRows.ColumnCount);

                        foreach (var pivot in target.ColumnPivots)
                        {
                            if (pivot.DeclaredColumn != null)
                            {
                                var value = row.Items[pivot.DeclaredOrder];

                                oldRow.Items[pivot.TargetOrder] = value;
                                newRow.Items[pivot.TargetOrder] = values[pivot.DeclaredOrder] ?? value;
                            }
                            else
                            {
                                oldRow.Items[pivot.TargetOrder] = QsiDataValue.Unknown;
                                newRow.Items[pivot.TargetOrder] = QsiDataValue.Unknown;
                            }
                        }

                        target.UpdateBeforeRows.Add(oldRow);
                        target.UpdateAfterRows.Add(newRow);
                    }

                    QsiTableColumn[] affectedColumns = target.ColumnPivots
                        .Where(p => p.DeclaredColumn != null && affectedColumnMap[p.DeclaredOrder])
                        .Select(p => p.DeclaredColumn)
                        .ToArray();

                    return new QsiDataManipulationResult
                    {
                        Table = target.Table,
                        AffectedColumns = affectedColumns,
                        UpdateBeforeRows = target.UpdateBeforeRows.ToNullIfEmpty(),
                        UpdateAfterRows = target.UpdateAfterRows.ToNullIfEmpty()
                    };
                })
                .ToArray<IQsiAnalysisResult>();
        }

        protected int FindColumnIndex(IAnalyzerContext context, QsiTableStructure table, QsiQualifiedIdentifier identifier)
        {
            var alias = new QsiQualifiedIdentifier(identifier[..^1]);
            var name = identifier[^1];

            var queue = new Queue<QsiTableColumn>();
            var recursiveTracker = new HashSet<QsiTableColumn>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (!IdentifierComparer.Equals(table.Columns[i].Name, name))
                    continue;

                if (identifier.Level == 1)
                    return i;

                queue.Clear();
                recursiveTracker.Clear();

                queue.Enqueue(table.Columns[i]);

                while (queue.TryDequeue(out var column))
                {
                    recursiveTracker.Add(column);

                    if (column.Parent.HasIdentifier)
                    {
                        // * case - Explicit access
                        if (QualifiedIdentifierComparer.Equals(column.Parent.Identifier, alias))
                            return i;

                        // * case - N Level implicit access
                        if (context.Options.UseExplicitRelationAccess)
                            break;

                        if (!QsiUtility.IsReferenceType(column.Parent.Type))
                            continue;

                        if (column.Parent.Identifier.Level <= identifier.Level)
                            break;

                        QsiIdentifier[] partialIdentifiers = column.Parent.Identifier[^identifier.Level..];
                        var partialIdentifier = new QsiQualifiedIdentifier(partialIdentifiers);

                        if (QualifiedIdentifierComparer.Equals(partialIdentifier, alias))
                            return i;

                        break;
                    }

                    foreach (var refColumn in column.References.Where(refColumn => !recursiveTracker.Contains(refColumn)))
                    {
                        queue.Enqueue(refColumn);
                    }
                }
            }

            return -1;
        }
        #endregion

        #region SearchPath
        protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteSearchPathAction(IAnalyzerContext context, IQsiChangeSearchPathActionNode action)
        {
            var fakeRefIdentifier = new QsiIdentifier(string.Empty, false);

            QsiQualifiedIdentifier[] identifiers = action.Identifiers
                .Select(identifier =>
                {
                    var qualifiedIdentifier = new QsiQualifiedIdentifier(identifier.Append(fakeRefIdentifier));
                    qualifiedIdentifier = context.Engine.RepositoryProvider.ResolveQualifiedIdentifier(qualifiedIdentifier);
                    return new QsiQualifiedIdentifier(qualifiedIdentifier[..^1]);
                })
                .ToArray();

            return new QsiChangeSearchPathAction(identifiers).ToSingleArray().AsValueTask();
        }
        #endregion
    }
}
