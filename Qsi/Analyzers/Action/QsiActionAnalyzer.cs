using System;
using System.Collections.Generic;
using System.Data;
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
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Analyzers.Action
{
    public class QsiActionAnalyzer : QsiAnalyzerBase
    {
        protected delegate QsiDataValue DataValueSelector(TableDataColumnPivot pivot);

        public QsiActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return tree is IQsiActionNode;
        }

        protected override async ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context)
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
        protected virtual ValueTask<IQsiAnalysisResult> PrepareAction(IAnalyzerContext context, IQsiPrepareActionNode action)
        {
            string query;

            switch (action.Query)
            {
                case IQsiLiteralExpressionNode literal when literal.Type == QsiDataType.String:
                    query = literal.Value?.ToString();
                    break;

                case IQsiVariableAccessExpressionNode variableAccess:
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

        protected virtual ValueTask<IQsiAnalysisResult> DropPrepareAction(IAnalyzerContext context, IQsiDropPrepareActionNode action)
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

        protected virtual ValueTask<IQsiAnalysisResult> ExecuteAction(IAnalyzerContext context, IQsiExecutePrepareActionNode action)
        {
            var definition = context.Engine.RepositoryProvider.LookupDefinition(action.Identifier, QsiTableType.Prepared) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, action.Identifier);

            return context.Engine.Execute(definition);
        }
        #endregion

        #region Insert, Replace
        private async ValueTask<IQsiAnalysisResult> DataInsertAction(IAnalyzerContext context, IQsiDataInsertActionNode action)
        {
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new TableCompileContext(context))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);
            }

            QsiIdentifier[] columnNames = ResolveColumnNames(table, action);

            var dataContext = new TableDataInsertContext(context, table)
            {
                ColumnNames = columnNames,
                Targets = ResolveDataTargets(table, columnNames)
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

            IQsiAction[] result = dataContext.Targets
                .Select(t => new QsiDataAction
                {
                    Table = t.Table,
                    InsertRows = ToNullIfEmpty(t.InsertRows),
                    DuplicateRows = ToNullIfEmpty(t.DuplicateRows)
                })
                .OfType<IQsiAction>()
                .ToArray();

            return new QsiActionAnalysisResult(new QsiActionSet(result));

            static QsiDataRowCollection ToNullIfEmpty(QsiDataRowCollection collection)
            {
                if (collection?.Count > 0)
                    return collection;

                return null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiIdentifier[] ResolveColumnNames(QsiTableStructure table, IQsiDataInsertActionNode action)
        {
            if (!ListUtility.IsNullOrEmpty(action.Columns))
            {
                return action.Columns;
            }

            if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                return ResolveSetColumnsPivot(action.SetValues).ColumnNames;
            }

            return table.Columns
                .Select(c => c.Name)
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual TableDataInsertTarget[] ResolveDataTargets(QsiTableStructure table, IEnumerable<QsiIdentifier> columnNames)
        {
            return columnNames
                .Select((declaredName, i) =>
                {
                    var index = table.Columns.FindIndex(c => Match(c.Name, declaredName));

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, declaredName);

                    return ResolveColumnPivot(table.Columns[index], i);
                })
                .GroupBy(c => c.TargetColumn.Parent)
                .Select(g =>
                {
                    var buffer = new TableDataColumnPivot[g.Key.Columns.Count];

                    foreach (var pivot in g)
                        buffer[pivot.TargetOrder] = pivot;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (buffer[i] != default)
                            continue;

                        buffer[i] = new TableDataColumnPivot(i, g.Key.Columns[i], -1, null);
                    }

                    return new TableDataInsertTarget(g.Key, buffer);
                })
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual SetColumnsPivot ResolveSetColumnsPivot(IQsiSetColumnExpressionNode[] setValues)
        {
            int count = setValues
                .Select(t => t.Target[^1])
                .Distinct(IdentifierComparer)
                .Count();

            // Use last affected columns
            // A, B, A, B, B
            //       ^     ^
            var set = new HashSet<QsiIdentifier>(IdentifierComparer);
            int index = setValues.Length;

            var columnNames = new QsiIdentifier[count];
            var affectedIndices = new int[count];

            foreach (var columnName in setValues.Reverse().Select(v => v.Target[^1]))
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual TableDataColumnPivot ResolveColumnPivot(QsiTableColumn declaredColumn, int declaredOrder)
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

                recursiveTracing ??= new HashSet<QsiTableColumn>();
                recursiveTracing.Add(column);

                column = column.References[0];
            }

            return new TableDataColumnPivot(
                column.Parent.Columns.IndexOf(column),
                column,
                declaredOrder,
                declaredColumn);
        }

        private async ValueTask ProcessQueryValues(TableDataInsertContext context, IQsiTableDirectivesNode directives, IQsiTableNode valueTable)
        {
            var engine = context.Engine;
            var script = engine.TreeDeparser.Deparse(valueTable, context.Script);

            if (directives != null)
                script = $"{engine.TreeDeparser.Deparse(directives, context.Script)}\n{script}";

            var scriptType = engine.ScriptParser.GetSuitableType(script);
            var rows = await engine.RepositoryProvider.GetDataRows(new QsiScript(script, scriptType));

            if (rows.ColumnCount != context.ColumnNames.Length)
                throw new QsiException(QsiError.DifferentColumnsCount);

            foreach (var row in rows)
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
            var setColumnsPivot = ResolveSetColumnsPivot(setValues);

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

            var setColumnsPivot = ResolveSetColumnsPivot(action.SetValues);
            TableDataInsertTarget[] updateTargets = ResolveDataTargets(context.Table, setColumnsPivot.ColumnNames);

            foreach (var updateTarget in updateTargets)
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

        [MethodImpl(MethodImplOptions.NoInlining)]
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiDataValue ResolveDefaultColumnValue(TableDataColumnPivot pivot)
        {
            if (pivot.TargetColumn.Default != null)
                return new QsiDataValue(pivot.TargetColumn.Default, QsiDataType.Raw);

            return QsiDataValue.Default;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual QsiDataValue ResolveColumnValue(TableDataInsertContext context, IQsiExpressionNode expression)
        {
            if (expression is IQsiLiteralExpressionNode literal)
            {
                return new QsiDataValue(literal.Value, literal.Type);
            }

            var value = context.Engine.TreeDeparser.Deparse(expression, context.Script);

            return new QsiDataValue(value, QsiDataType.Raw);
        }
        #endregion
    }
}
