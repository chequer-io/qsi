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
    public partial class QsiActionAnalyzer : QsiAnalyzerBase
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

            var dataContext = new DataInsertContext(context, action, table);

            ProcessDataTargets(dataContext);

            if (action.ValueTable != null)
            {
                await ProcessQueryValues(dataContext);
            }
            else if (!ListUtility.IsNullOrEmpty(action.Values))
            {
                ProcessValues(dataContext);
            }
            else if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                ProcessSetValues(dataContext);
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            IQsiAction[] result = dataContext.Targets
                .Select(t => new QsiDataAction
                {
                    Table = t.Table,
                    InsertRows = t.Rows
                })
                .OfType<IQsiAction>()
                .ToArray();

            return new QsiActionAnalysisResult(new QsiActionSet(result));
        }

        private void ProcessDataTargets(DataInsertContext context)
        {
            if (!ListUtility.IsNullOrEmpty(context.Action.Columns))
            {
                context.ColumnsNames = context.Action.Columns;
            }
            else if (!ListUtility.IsNullOrEmpty(context.Action.SetValues))
            {
                int count = context.Action.SetValues
                    .Select(t => t.Target[^1])
                    .Distinct(IdentifierComparer)
                    .Count();

                // Use last affected columns
                // A, B, A, B, B
                //       ^     ^
                var set = new HashSet<QsiIdentifier>(IdentifierComparer);
                int index = context.Action.SetValues.Length;

                context.ColumnsNames = new QsiIdentifier[count];
                context.AffectedIndices = new int[count];

                foreach (var columnName in context.Action.SetValues.Reverse().Select(v => v.Target[^1]))
                {
                    index--;

                    if (set.Contains(columnName))
                        continue;

                    set.Add(columnName);
                    context.ColumnsNames[count - set.Count] = columnName;
                    context.AffectedIndices[count - set.Count] = index;
                }
            }
            else
            {
                context.ColumnsNames = null;
            }

            var pivotColumns = new List<DataInsertColumnPivot>();

            if (context.ColumnsNames != null)
            {
                var indices = new int[context.ColumnsNames.Length];

                for (int i = 0; i < indices.Length; i++)
                {
                    ref int index = ref indices[i];
                    var declaredName = context.ColumnsNames[i];

                    index = context.Table.Columns.FindIndex(c => Match(c.Name, declaredName));

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, declaredName);

                    pivotColumns.Add(ResolveColumnPivot(context.Table.Columns[index], i));
                }

                context.ColumnsIndices = indices;
            }
            else
            {
                context.ColumnsIndices = new int[context.Table.Columns.Count];
                context.ColumnsNames = new QsiIdentifier[context.ColumnsIndices.Length];

                for (int i = 0; i < context.ColumnsIndices.Length; i++)
                {
                    context.ColumnsNames[i] = context.Table.Columns[i].Name;
                    context.ColumnsIndices[i] = i;

                    pivotColumns.Add(ResolveColumnPivot(context.Table.Columns[i], i));
                }
            }

            IEnumerable<DataInsertTargetContext> targets = pivotColumns
                .GroupBy(c => c.TargetColumn.Parent)
                .Select(g =>
                {
                    var buffer = new DataInsertColumnPivot[g.Key.Columns.Count];

                    foreach (var pivot in g)
                        buffer[pivot.TargetOrder] = pivot;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (buffer[i] != null)
                            continue;

                        buffer[i] = new DataInsertColumnPivot(i, g.Key.Columns[i], -1, null);
                    }

                    return new DataInsertTargetContext(g.Key, buffer);
                });

            context.Targets.AddRange(targets);
        }

        private DataInsertColumnPivot ResolveColumnPivot(QsiTableColumn declaredColumn, int declaredOrder)
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

            return new DataInsertColumnPivot(
                column.Parent.Columns.IndexOf(column),
                column,
                declaredOrder,
                declaredColumn);
        }

        private async ValueTask ProcessQueryValues(DataInsertContext context)
        {
            var script = Engine.TreeDeparser.Deparse(context.Action.ValueTable, context.ExecutionContext.Script);

            if (context.Action.Directives != null)
                script = $"{Engine.TreeDeparser.Deparse(context.Action.Directives, context.ExecutionContext.Script)}\n{script}";

            var scriptType = Engine.ScriptParser.GetSuitableType(script);
            var rows = await Engine.RepositoryProvider.GetDataRows(new QsiScript(script, scriptType));

            if (rows.ColumnCount != context.ColumnsNames.Length)
                throw new QsiException(QsiError.DifferentColumnsCount);

            foreach (var row in rows)
            {
                foreach (var target in context.Targets)
                {
                    var targetRow = target.Rows.NewRow();

                    foreach (var pivot in target.ColumnPivots)
                    {
                        if (pivot.DeclaredColumn != null)
                        {
                            targetRow.Items[pivot.TargetOrder] = row.Items[pivot.DeclaredOrder];
                        }
                        else if (pivot.TargetColumn.Default != null)
                        {
                            targetRow.Items[pivot.TargetOrder] = new QsiDataValue(pivot.TargetColumn.Default, QsiDataType.Raw);
                        }
                        else
                        {
                            targetRow.Items[pivot.TargetOrder] = new QsiDataValue(null, QsiDataType.Default);
                        }
                    }
                }
            }
        }

        private void ProcessValues(DataInsertContext context)
        {
            int columnCount = context.ColumnsNames.Length;

            for (int i = 0; i < context.Action.Values.Length; i++)
            {
                var value = context.Action.Values[i];

                if (columnCount != value.ColumnValues.Length)
                    throw new QsiException(QsiError.DifferentColumnValueCount, i + 1);

                foreach (var target in context.Targets)
                {
                    var targetRow = target.Rows.NewRow();

                    foreach (var pivot in target.ColumnPivots)
                    {
                        if (pivot.DeclaredColumn != null)
                        {
                            targetRow.Items[pivot.TargetOrder] = ResolveColumnValue(context, value.ColumnValues[pivot.DeclaredOrder]);
                        }
                        else if (pivot.TargetColumn.Default != null)
                        {
                            targetRow.Items[pivot.TargetOrder] = new QsiDataValue(pivot.TargetColumn.Default, QsiDataType.Raw);
                        }
                        else
                        {
                            targetRow.Items[pivot.TargetOrder] = new QsiDataValue(null, QsiDataType.Default);
                        }
                    }
                }
            }
        }

        private void ProcessSetValues(DataInsertContext context)
        {
            foreach (var target in context.Targets)
            {
                var targetRow = target.Rows.NewRow();

                foreach (var pivot in target.ColumnPivots)
                {
                    if (pivot.DeclaredColumn != null)
                    {
                        var affectedIndex = context.AffectedIndices[pivot.DeclaredOrder];
                        targetRow.Items[pivot.TargetOrder] = ResolveColumnValue(context, context.Action.SetValues[affectedIndex].Value);
                    }
                    else if (pivot.TargetColumn.Default != null)
                    {
                        targetRow.Items[pivot.TargetOrder] = new QsiDataValue(pivot.TargetColumn.Default, QsiDataType.Raw);
                    }
                    else
                    {
                        targetRow.Items[pivot.TargetOrder] = new QsiDataValue(null, QsiDataType.Default);
                    }
                }
            }
        }

        protected virtual QsiDataValue ResolveColumnValue(DataInsertContext context, IQsiExpressionNode expression)
        {
            if (expression is IQsiLiteralExpressionNode literal)
            {
                return new QsiDataValue(literal.Value, literal.Type);
            }

            var value = Engine.TreeDeparser.Deparse(expression, context.ExecutionContext.Script);

            return new QsiDataValue(value, QsiDataType.Raw);
        }
        #endregion
    }
}
