using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Analyzers.Action
{
    // 디버깅 없이 개발하는건 정신 나갈것 같다..
    public class QsiActionAnalyzer : QsiAnalyzerBase
    {
        public QsiActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return tree is IQsiActionNode;
        }

        protected override async ValueTask<IQsiAnalysisResult> OnExecute(ExecutionContext context)
        {
            switch (context.Tree)
            {
                case IQsiPrepareActionNode prepareActionNode:
                    return await PrepareAction(context, prepareActionNode);

                case IQsiDropPrepareActionNode dropPrepareAction:
                    return await DropPrepareAction(context, dropPrepareAction);

                case IQsiExecutePrepareActionNode executeAction:
                    return await ExecuteAction(context, executeAction);

                case IQsiDataInsertActionNode dataInsertAction:
                    return await DataInsertAction(context, dataInsertAction);

                default:
                    throw TreeHelper.NotSupportedTree(context.Tree);
            }
        }

        #region Prepared
        protected virtual ValueTask<IQsiAnalysisResult> PrepareAction(ExecutionContext context, IQsiPrepareActionNode action)
        {
            string query;

            switch (action.Query)
            {
                case IQsiLiteralExpressionNode literal when literal.Type == QsiDataType.String:
                    query = literal.Value?.ToString();
                    break;

                case IQsiVariableAccessExpressionNode variableAccess:
                    var variable = Engine.RepositoryProvider.LookupVariable(variableAccess.Identifier) ??
                                   throw new QsiException(QsiError.UnknownVariable, variableAccess.Identifier);

                    if (variable.Type != QsiDataType.String)
                        throw new InvalidOperationException();

                    query = variable.Value.ToString();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var scriptType = Engine.ScriptParser.GetSuitableType(query);

            var result = new QsiActionAnalysisResult(new QsiReferenceAction
            {
                Type = QsiReferenceType.Prepared,
                Operation = QsiReferenceOperation.Create,
                IsolationLevel = QsiReferenceIsolationLevel.Session,
                Target = action.Identifier,
                Definition = new QsiScript(query, scriptType)
            });

            return new ValueTask<IQsiAnalysisResult>(result);
        }

        protected virtual ValueTask<IQsiAnalysisResult> DropPrepareAction(ExecutionContext context, IQsiDropPrepareActionNode action)
        {
            var result = new QsiActionAnalysisResult(new QsiReferenceAction
            {
                Type = QsiReferenceType.Prepared,
                Operation = QsiReferenceOperation.Delete,
                IsolationLevel = QsiReferenceIsolationLevel.Session,
                Target = action.Identifier
            });

            return new ValueTask<IQsiAnalysisResult>(result);
        }

        protected virtual ValueTask<IQsiAnalysisResult> ExecuteAction(ExecutionContext context, IQsiExecutePrepareActionNode action)
        {
            var definition = Engine.RepositoryProvider.LookupDefinition(action.Identifier, QsiTableType.Prepared) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, action.Identifier);

            return Engine.Execute(definition);
        }
        #endregion

        #region Insert, Replace
        private async ValueTask<IQsiAnalysisResult> DataInsertAction(ExecutionContext context, IQsiDataInsertActionNode action)
        {
            var tableAnalyzer = Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new QsiTableAnalyzer.CompileContext(context.Options, context.CancellationToken))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target).ConfigureAwait(false);
            }

            UpdateTarget[] targets = BuildUpdateTargets(action, table);
            QsiDataRowCollection rows;

            if (action.ValueTable != null)
            {
                var script = Engine.TreeDeparser.Deparse(action.ValueTable);

                if (action.Directives != null)
                    script = $"{Engine.TreeDeparser.Deparse(action.Directives)}\n{script}";

                var scriptType = Engine.ScriptParser.GetSuitableType(script);
                rows = Engine.RepositoryProvider.GetDataRows(new QsiScript(script, scriptType));
            }
            else if (!ListUtility.IsNullOrEmpty(action.Values))
            {
                rows = new QsiDataRowCollection(action.Columns.Count, action.Values.Length);
                rows.AddRange(action.Values.Select(e => BuildDataRow(e.ColumnValues)));
            }
            else if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                rows = new QsiDataRowCollection(action.Columns.Count)
                {
                    BuildDataRow(action.SetValues.Select(e => e.Value).ToArray())
                };
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            if (rows.ColumnCount != action.Columns.Count)
                throw new QsiException(QsiError.SpecifiesMoreColumnNames);

            var dataActions = new QsiDataAction[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                var targetColumnLength = target.Columns.Length;
                var rowDatas = new (QsiDataRow OldRow, QsiDataRow NewRow)[rows.Count];
                var indices = new Dictionary<int, int>();

                for (int j = 0; j < rows.Count; j++)
                {
                    var values = new QsiDataValue[targetColumnLength];

                    for (int k = 0; k < targetColumnLength; k++)
                    {
                        int order = target.Columns[k].DeclaredOrder;

                        if (order >= 0)
                        {
                            indices[order] = k;
                            values[k] = rows[j].Items[order];
                        }
                        else
                        {
                            // TODO: get default value of column
                        }

                        rowDatas[j] = (null, new QsiDataRow(values));
                    }
                }

                UpdateColumnTarget[] uniqueColumns = target.Columns
                    .Where(c => c.Column.IsUnique)
                    .ToArray();

                // TODO: Impl PK, UI
                // foreach (var uniqueColumn in uniqueColumns)
                // {
                //     if (indices.TryGetValue(uniqueColumn.DeclaredOrder, out var index))
                //     {
                //         QsiDataValue[] uniqueKeys = rowDatas
                //             .Select(r => r.NewRow.Items[index])
                //             .ToArray();
                //
                //         // TODO: GetConflictUniqueKeys(uniqueKeys)
                //         var conflictKeys = new QsiDataValue[0];
                //
                //         if (conflictKeys != null)
                //         {
                //             for (int j = 0; j < uniqueKeys.Length; j++)
                //             {
                //                 if (!conflictKeys.Contains(uniqueKeys[j]))
                //                     continue;
                //
                //                 // rowDatas[j].OldRow
                //                 // TODO: rowDatas[j].Row.Items = /* action.ConflictAction */ 
                //             }
                //         }
                //     }
                //     else
                //     {
                //         // TODO: Check auto inc
                //     }
                // }

                QsiDataRowCollection updateBeforeRows = null;
                QsiDataRowCollection updateAfterRows = null;
                QsiDataRowCollection insertRows = null;

                foreach (var (oldRow, newRow) in rowDatas)
                {
                    if (oldRow != null)
                    {
                        updateBeforeRows ??= new QsiDataRowCollection(targetColumnLength);
                        updateBeforeRows.Add(oldRow);

                        updateAfterRows ??= new QsiDataRowCollection(targetColumnLength);
                        updateAfterRows.Add(newRow);
                    }
                    else
                    {
                        insertRows ??= new QsiDataRowCollection(targetColumnLength);
                        insertRows.Add(newRow);
                    }
                }

                dataActions[i] = new QsiDataAction
                {
                    Table = target.Table,
                    UpdateBeforeRows = updateBeforeRows,
                    UpdateAfterRows = updateAfterRows,
                    InsertRows = insertRows
                };
            }

            // TODO: analyze data rows

            var result = new QsiDataActionSet<QsiDataAction>(dataActions);

            return new QsiActionAnalysisResult(result);
        }

        private UpdateTarget[] BuildUpdateTargets(IQsiDataInsertActionNode action, QsiTableStructure table)
        {
            IEnumerable<UpdateColumnTarget> targetColumns;
            QsiIdentifier[] declaredColumnNames = null;

            if (action.Columns != null)
            {
                declaredColumnNames = GetDeclaredColumnNames(action.Columns);
            }
            else if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                declaredColumnNames = GetDeclaredColumnNames(action.SetValues);
            }

            if (declaredColumnNames != null)
            {
                if (declaredColumnNames.Length > table.Columns.Count)
                    throw new QsiException(QsiError.SpecifiesMoreColumnNames);

                List<QsiTableColumn> buffer = table.Columns.ToList();
                var result = new UpdateColumnTarget[declaredColumnNames.Length];

                for (int i = 0; i < declaredColumnNames.Length; i++)
                {
                    var name = declaredColumnNames[i];
                    var index = buffer.FindIndex(c => Match(c.Name, name));

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, name.Value);

                    result[i] = ResolveColumnTarget(buffer[index], i);
                    buffer.RemoveAt(index);
                }

                targetColumns = result;
            }
            else
            {
                targetColumns = table.Columns.Select(ResolveColumnTarget);
            }

            return targetColumns
                .GroupBy(c => c.Column.Parent)
                .Select(g =>
                {
                    IReadOnlyList<QsiTableColumn> tableColumns = g.Key.Columns;
                    var columnTargets = new UpdateColumnTarget[tableColumns.Count];

                    foreach (var columnTarget in g)
                    {
                        var index = tableColumns.IndexOf(columnTarget.Column);
                        columnTargets[index] = columnTarget;
                    }

                    for (int i = 0; i < columnTargets.Length; i++)
                    {
                        if (columnTargets[i].Column == null)
                        {
                            columnTargets[i] = new UpdateColumnTarget(tableColumns[i]);
                        }
                    }

                    return new UpdateTarget(g.Key, columnTargets);
                })
                .ToArray();
        }

        private UpdateColumnTarget ResolveColumnTarget(QsiTableColumn declaredColumn, int declaredOrder)
        {
            if (declaredColumn.IsAnonymous)
                throw new QsiException(QsiError.NotUpdatableColumn, "anonymous");

            if (declaredColumn.IsBinding || declaredColumn.IsExpression || !declaredColumn.IsVisible)
                throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

            var column = declaredColumn;
            HashSet<QsiTableColumn> recursiveTracing = null;

            while (column.Parent.Type != QsiTableType.Table)
            {
                if (column.References.Count != 1)
                    throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

                if (recursiveTracing?.Contains(column) ?? false)
                    throw new QsiException(QsiError.NotUpdatableColumn, declaredColumn.Name);

                column = column.References[0];

                recursiveTracing ??= new HashSet<QsiTableColumn>();
                recursiveTracing.Add(column);
            }

            return new UpdateColumnTarget(declaredOrder, declaredColumn, column);
        }

        private QsiIdentifier[] GetDeclaredColumnNames(IQsiColumnsDeclarationNode columnsDeclaration)
        {
            var columns = new QsiIdentifier[columnsDeclaration.Count];

            for (int i = 0; i < columns.Length; i++)
            {
                var c = columnsDeclaration.Columns[i];

                if (c is IQsiDeclaredColumnNode column)
                {
                    if (column.Name.Level != 1)
                        throw new QsiException(QsiError.Internal, $"Invalid column name '{column.Name}'");

                    columns[i] = column.Name[0];
                    continue;
                }

                throw new QsiException(QsiError.Internal, $"Not supported column type '{c.GetType().Name}'");
            }

            return columns;
        }

        private QsiIdentifier[] GetDeclaredColumnNames(IQsiSetColumnExpressionNode[] setColumns)
        {
            var columns = new QsiIdentifier[setColumns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                var target = setColumns[i].Target;

                if (target.Level != 1)
                    throw new QsiException(QsiError.Internal, $"Invalid column name '{target}'");

                columns[i] = target[0];
            }

            return columns;
        }

        protected virtual QsiDataRow BuildDataRow(IQsiExpressionNode[] expressions)
        {
            var row = new QsiDataRow(expressions.Length);

            for (int i = 0; i < expressions.Length; i++)
            {
                row.Items[i] = ResolveColumnValue(expressions[i]);
            }

            return row;
        }

        protected virtual QsiDataValue ResolveColumnValue(IQsiExpressionNode expression)
        {
            throw new NotImplementedException();
        }
        #endregion

        private readonly struct UpdateTarget
        {
            public QsiTableStructure Table { get; }

            public UpdateColumnTarget[] Columns { get; }

            public UpdateTarget(QsiTableStructure table, UpdateColumnTarget[] columns)
            {
                if (table.Type != QsiTableType.Table)
                    throw new NotSupportedException(table.Type.ToString());

                Table = table;
                Columns = columns;
            }
        }

        private readonly struct UpdateColumnTarget
        {
            public int DeclaredOrder { get; }

            public QsiTableColumn DeclaredColumn { get; }

            public QsiTableColumn Column { get; }

            public bool IsDeclared => DeclaredColumn != null;

            public UpdateColumnTarget(QsiTableColumn column) : this(-1, null, column)
            {
            }

            public UpdateColumnTarget(int declaredOrder, QsiTableColumn declaredColumn, QsiTableColumn column)
            {
                if (column.Parent.Type != QsiTableType.Table)
                    throw new ArgumentException(nameof(column));

                DeclaredOrder = declaredOrder;
                DeclaredColumn = declaredColumn;
                Column = column;
            }
        }

        private enum RowAction
        {
            None,
            Insert,
            Update,
            Delete
        }
    }
}
