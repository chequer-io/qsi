using System;
using System.Threading.Tasks;
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
                    return await PrepareAction(prepareActionNode);

                case IQsiDropPrepareActionNode dropPrepareAction:
                    return await DropPrepareAction(dropPrepareAction);

                case IQsiExecuteActionNode executeAction:
                    return await ExecuteAction(executeAction);

                default:
                    throw TreeHelper.NotSupportedTree(context.Tree);
            }
        }

        protected virtual ValueTask<IQsiAnalysisResult> PrepareAction(IQsiPrepareActionNode node)
        {
            string query;

            switch (node.Query)
            {
                case IQsiLiteralExpressionNode literal when literal.Type == QsiDataType.String:
                    query = literal.Value?.ToString();
                    break;

                case IQsiVariableAccessExpressionNode variableAccess:
                    var variable = Engine.ReferenceResolver.LookupVariable(variableAccess.Identifier) ??
                                   throw new QsiException(QsiError.UnknownVariable, variableAccess.Identifier);

                    if (variable.Type != QsiDataType.String)
                        throw new InvalidOperationException();

                    query = variable.Value.ToString();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var result = new QsiActionAnalysisResult(new QsiAction
            {
                Type = QsiActionType.AllocatePreparedQuery,
                Target = node.Identifier,
                Payload = query
            });

            return new ValueTask<IQsiAnalysisResult>(result);
        }

        protected virtual ValueTask<IQsiAnalysisResult> DropPrepareAction(IQsiDropPrepareActionNode node)
        {
            var result = new QsiActionAnalysisResult(new QsiAction
            {
                Type = QsiActionType.DeallocatePreparedQUery,
                Target = node.Identifier
            });

            return new ValueTask<IQsiAnalysisResult>(result);
        }

        protected virtual ValueTask<IQsiAnalysisResult> ExecuteAction(IQsiExecuteActionNode node)
        {
            var definition = Engine.ReferenceResolver.LookupDefinition(node.Identifier, QsiTableType.Prepared) ??
                             throw new QsiException(QsiError.UnableResolveDefinition, node.Identifier);

            return Engine.Execute(definition);
        }
    }
}
