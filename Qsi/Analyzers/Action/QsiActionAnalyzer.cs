using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Qsi.Analyzers.Action.Context;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Shared.Extensions;
using Qsi.Tree;
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
        protected virtual QsiDataValue ResolveColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
        {
            if (expression is IQsiLiteralExpressionNode literal)
            {
                return new QsiDataValue(literal.Value, literal.Type);
            }

            var value = context.Engine.TreeDeparser.Deparse(expression, context.Script);

            return new QsiDataValue(value, QsiDataType.Raw);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(QsiTableStructure table)
        {
            return ResolveDataManipulationTargetsCore(table.Columns.SelectMany(ResolveDataManipulationTargetColumns));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(QsiTableStructure table, IEnumerable<QsiQualifiedIdentifier> columnNames)
        {
            QsiTableColumn[] columnsBuffer = table.Columns.ToArray();

            IEnumerable<DataManipulationTargetColumnPivot> rawPivots = columnNames
                .SelectMany((declaredName, i) =>
                {
                    var index = columnsBuffer.FindIndex(c => c != null && Match(c.Name, declaredName[^1]));

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, declaredName);

                    columnsBuffer[index] = null;
                    return ResolveDataManipulationTargetColumns(table.Columns[index], i);
                });

            return ResolveDataManipulationTargetsCore(rawPivots);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargetsCore(IEnumerable<DataManipulationTargetColumnPivot> rawPivots)
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

                    return new DataManipulationTarget(g.Key, buffer);
                });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual IEnumerable<DataManipulationTargetColumnPivot> ResolveDataManipulationTargetColumns(QsiTableColumn declaredColumn, int declaredOrder)
        {
            if (declaredColumn.IsAnonymous)
                throw new QsiException(QsiError.NotUpdatableColumn, "anonymous");

            if (declaredColumn.IsBinding || declaredColumn.IsExpression || !declaredColumn.IsVisible)
                throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

            var queue = new Queue<QsiTableColumn>();
            queue.Enqueue(declaredColumn);

            HashSet<QsiTableColumn> recursiveTracing = null;

            while (queue.TryDequeue(out var column))
            {
                bool skip = false;

                recursiveTracing?.Clear();

                while (column.Parent.Type != QsiTableType.Table)
                {
                    var refCount = column.References.Count;

                    if (column.Parent.Type == QsiTableType.Join && refCount == 0 || refCount != 1)
                        throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

                    if (recursiveTracing?.Contains(column) ?? false)
                        throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

                    recursiveTracing ??= new HashSet<QsiTableColumn>();
                    recursiveTracing.Add(column);

                    if (column.Parent.Type is QsiTableType.Join or QsiTableType.Union)
                    {
                        foreach (var joinedColumn in column.References)
                        {
                            queue.Enqueue(joinedColumn);
                        }

                        skip = true;
                        break;
                    }

                    column = column.References[0];
                }

                if (skip)
                    continue;

                yield return new DataManipulationTargetColumnPivot(
                    column.Parent.Columns.IndexOf(column),
                    column,
                    declaredOrder,
                    declaredColumn);
            }
        }

        protected virtual QsiTableColumn[] GetAffectedColumns(DataManipulationTarget target)
        {
            return target.ColumnPivots
                .Select(p => p.DeclaredColumn)
                .Where(c => c != null)
                .ToArray();
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

            return await context.Engine.RepositoryProvider.GetDataTable(script, parameters);
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

            QsiQualifiedIdentifier[] columnNames = ResolveColumnNames(table, action);

            var dataContext = new TableDataInsertContext(context, table)
            {
                ColumnNames = columnNames,
                Targets = ResolveDataManipulationTargets(table, columnNames).ToArray()
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
                .Select(t => new QsiDataAction
                {
                    Table = t.Table,
                    AffectedColumns = GetAffectedColumns(t),
                    InsertRows = t.InsertRows.ToNullIfEmpty(),
                    DuplicateRows = t.DuplicateRows.ToNullIfEmpty()
                })
                .ToArray<IQsiAnalysisResult>();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiQualifiedIdentifier[] ResolveColumnNames(QsiTableStructure table, IQsiDataInsertActionNode action)
        {
            if (!ListUtility.IsNullOrEmpty(action.Columns))
            {
                return action.Columns;
            }

            if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                return ResolveSetColumnsPivot(action.SetValues, true).Columns
                    .Select(n => n)
                    .ToArray();
            }

            return table.Columns
                .Select(c => new QsiQualifiedIdentifier(c.Name))
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual SetColumnsPivot ResolveSetColumnsPivot(IQsiSetColumnExpressionNode[] setValues, bool ignoreAlias)
        {
            int count = setValues
                .Select(t => t.Target[^1])
                .Distinct(IdentifierComparer)
                .Count();

            IEqualityComparer<QsiQualifiedIdentifier> equalityComparer = ignoreAlias ?
                new DelegateEqualityComparer<QsiQualifiedIdentifier>((x, y) => Match(x[^1], y[^1])) :
                QualifiedIdentifierComparer;

            // Use last affected columns
            // A, B, A, B, B
            //       ^     ^
            var set = new HashSet<QsiQualifiedIdentifier>(equalityComparer);
            int index = setValues.Length;

            var columnNames = new QsiQualifiedIdentifier[count];
            var affectedIndices = new int[count];

            foreach (var columnName in setValues.Reverse().Select(v => v.Target))
            {
                index--;

                if (set.Contains(columnName))
                    continue;

                set.Add(columnName);
                columnNames[count - set.Count] = columnName;
                affectedIndices[count - set.Count] = index;
            }

            return new SetColumnsPivot(columnNames, affectedIndices);
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
            var dataTable = await engine.RepositoryProvider.GetDataTable(new QsiScript(script, scriptType), parameters);

            if (dataTable.Rows.ColumnCount != context.ColumnNames.Length)
                throw new QsiException(QsiError.DifferentColumnsCount);

            foreach (var row in dataTable.Rows)
            {
                PopulateInsertRow(context, pivot => row.Items[pivot.DeclaredOrder]);
            }
        }

        private void ProcessValues(TableDataInsertContext context, IQsiRowValueExpressionNode[] values)
        {
            int columnCount = context.ColumnNames.Length;

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
            var setColumnsPivot = ResolveSetColumnsPivot(setValues, true);

            PopulateInsertRow(context, pivot =>
            {
                var affectedIndex = setColumnsPivot.AffectedIndices[pivot.DeclaredOrder];
                return ResolveColumnValue(context, setValues[affectedIndex].Value);
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

            var setColumnsPivot = ResolveSetColumnsPivot(action.SetValues, true);

            foreach (var updateTarget in ResolveDataManipulationTargets(context.Table, setColumnsPivot.Columns))
            {
                var target = context.Targets.FirstOrDefault(t => t.Table == updateTarget.Table);

                if (target == null)
                    continue;

                var targetRow = target.DuplicateRows.NewRow();

                foreach (var pivot in updateTarget.ColumnPivots)
                {
                    if (pivot.DeclaredColumn != null)
                    {
                        var affectedIndex = setColumnsPivot.AffectedIndices[pivot.DeclaredOrder];
                        targetRow.Items[pivot.TargetOrder] = ResolveColumnValue(context, action.SetValues[affectedIndex].Value);
                    }
                    else
                    {
                        targetRow.Items[pivot.TargetOrder] = QsiDataValue.Unset;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopulateInsertRow(TableDataInsertContext context, DataValueSelector valueSelector)
        {
            foreach (var target in context.Targets)
            {
                var targetRow = target.InsertRows.NewRow();

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
            }
        }
        #endregion

        #region Delete
        protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;
            int tableColumnCount;

            using (var tableContext = new TableCompileContext(context))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);
                tableColumnCount = table.Columns.Count;
            }

            var commonTableNode = ReassembleCommonTableNode(action.Target);
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

            return ResolveDataManipulationTargets(table, columnNames)
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        var targetRow = target.DeleteRows.NewRow();

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
                    }

                    return new QsiDataAction
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

            // values
            var values = new QsiDataValue[sourceTable.Columns.Count];
            var affectedColumnMap = new bool[sourceTable.Columns.Count];

            if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                // columns pivot
                var setColumnsPivot = ResolveSetColumnsPivot(action.SetValues, false);

                for (int i = 0; i < setColumnsPivot.Columns.Length; i++)
                {
                    int sourceIndex = FindColumnIndex(context, sourceTable, setColumnsPivot.Columns[i]);
                    var affectedIndex = setColumnsPivot.AffectedIndices[i];

                    values[sourceIndex] = ResolveColumnValue(context, action.SetValues[affectedIndex].Value);
                    affectedColumnMap[sourceIndex] = true;
                }
            }
            else if (action.Value != null)
            {
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

            return ResolveDataManipulationTargets(sourceTable)
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        var oldRow = target.UpdateBeforeRows.NewRow();
                        var newRow = target.UpdateAfterRows.NewRow();

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
                    }

                    QsiTableColumn[] affectedColumns = target.ColumnPivots
                        .Where(p => p.DeclaredColumn != null && affectedColumnMap[p.DeclaredOrder])
                        .Select(p => p.DeclaredColumn)
                        .ToArray();

                    return new QsiDataAction
                    {
                        Table = target.Table,
                        AffectedColumns = affectedColumns,
                        UpdateBeforeRows = target.UpdateBeforeRows.ToNullIfEmpty(),
                        UpdateAfterRows = target.UpdateAfterRows.ToNullIfEmpty()
                    };
                })
                .ToArray<IQsiAnalysisResult>();
        }

        private int FindColumnIndex(IAnalyzerContext context, QsiTableStructure table, QsiQualifiedIdentifier identifier)
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
                            break;

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
                    var qualifiedIdentifier = new QsiQualifiedIdentifier(identifier, fakeRefIdentifier);
                    qualifiedIdentifier = context.Engine.RepositoryProvider.ResolveQualifiedIdentifier(qualifiedIdentifier);
                    return new QsiQualifiedIdentifier(qualifiedIdentifier[..^1]);
                })
                .ToArray();

            return new QsiChangeSearchPathAction(identifiers).ToSingleArray().AsValueTask();
        }
        #endregion
    }
}
