using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Analyzers.Action
{
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
            var options = context.Options.Clone();
            options.UseViewTracing = false;

            var tableAnalyzer = Engine.GetAnalyzer<QsiTableAnalyzer>();
            QsiTableStructure table;

            using (var tableContext = new QsiTableAnalyzer.CompileContext(options, context.CancellationToken))
            {
                table = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);
            }

            var dataAction = new QsiDataAction
            {
                Table = table
            };

            QsiTableColumn[] targetColumns;

            if (action.Columns != null)
            {
                if (action.Columns.Columns.Length > table.Columns.Count)
                    throw new QsiException(QsiError.SpecifiesMoreColumnNames);

                List<QsiTableColumn> buffer = table.Columns.ToList();

                QsiIdentifier[] declaredColumns = GetDeclaredColumnNames(action.Columns);
                targetColumns = new QsiTableColumn[declaredColumns.Length];

                for (int i = 0; i < declaredColumns.Length; i++)
                {
                    var declaredColumn = declaredColumns[i];
                    var index = buffer.FindIndex(c => Match(c.Name, declaredColumn));

                    if (index == -1)
                        throw new QsiException(QsiError.UnknownColumn, declaredColumn.Value);

                    targetColumns[i] = buffer[index];
                    buffer.RemoveAt(index);
                }
            }
            else
            {
                targetColumns = table.Columns.ToArray();
            }

            QsiDataRowCollection rows;

            if (action.ValueTable != null)
            {
                var script = Engine.TreeDeparser.Deparse(action.ValueTable);

                if (action.Directives != null)
                {
                    script = $"{Engine.TreeDeparser.Deparse(action.Directives)}\n{script}";
                }

                rows = Engine.RepositoryProvider.GetDataRows(script);
            }
            else if (!ListUtility.IsNullOrEmpty(action.Values))
            {
                rows = BuildDataRows(action.Values);
            }
            else if (!ListUtility.IsNullOrEmpty(action.SetValues))
            {
                rows = BuildDataRow(action.SetValues);
            }
            else
            {
                throw new NotImplementedException();
            }

            if (targetColumns.Length != rows.Count)
                throw new QsiException(QsiError.DifferentColumnsCount);

            // TODO: analyze data rows

            return new QsiActionAnalysisResult(dataAction);
        }

        private QsiDataRowCollection BuildDataRows(IQsiRowValueExpressionNode[] values)
        {
            throw new NotImplementedException();
        }

        private QsiDataRowCollection BuildDataRow(IQsiSetColumnExpressionNode[] values)
        {
            throw new NotImplementedException();
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
        #endregion
    }
}
