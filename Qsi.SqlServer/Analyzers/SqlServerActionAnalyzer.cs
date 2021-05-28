using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer.Analyzers
{
    public class SqlServerActionAnalyzer : QsiActionAnalyzer
    {
        public SqlServerActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context)
        {
            switch (context.Tree)
            {
                case SqlServerMergeActionNode mergeActionNode:
                    return await ExecuteMergeAction(context, mergeActionNode);

                case SqlServerAlterUserActionNode alterUserActionNode:
                    return await ExecuteAlterUserAction(context, alterUserActionNode);
            }

            return await base.OnExecute(context);
        }

        protected ValueTask<IQsiAnalysisResult> ExecuteMergeAction(IAnalyzerContext context, SqlServerMergeActionNode mergeActionNode)
        {
            var actionSet = new QsiActionSet<QsiDataAction>();

            foreach (var actionNode in mergeActionNode.ActionNodes)
            {
                IQsiAnalysisResult result;

                switch (actionNode)
                {
                    case QsiDataInsertActionNode insertActionNode:
                    {
                        result = ExecuteDataInsertAction(context, insertActionNode).Result;
                        break;
                    }

                    case QsiDataDeleteActionNode deleteActionNode:
                    {
                        result = ExecuteDataDeleteAction(context, deleteActionNode).Result;
                        break;
                    }

                    case QsiDataUpdateActionNode updateActionNode:
                    {
                        result = ExecuteDataUpdateAction(context, updateActionNode).Result;
                        break;
                    }

                    default:
                    {
                        continue;
                    }
                }

                if (result is QsiActionAnalysisResult { Action: IQsiActionSet<QsiDataAction> dataActions })
                {
                    actionSet.AddRange(dataActions);
                }
            }

            return new ValueTask<IQsiAnalysisResult>(new QsiActionAnalysisResult(actionSet));
        }

        protected ValueTask<IQsiAnalysisResult> ExecuteAlterUserAction(IAnalyzerContext context, SqlServerAlterUserActionNode alterUserActionNode)
        {
            var action = new SqlServerAlterUserAction
            {
                TargetUser = alterUserActionNode.TargetUser,
                NewUserName = alterUserActionNode.NewUserName,
                DefaultSchema = alterUserActionNode.DefaultSchema
            };

            return new ValueTask<IQsiAnalysisResult>(new QsiActionAnalysisResult(action));
        }

        protected override IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
        {
            switch (node)
            {
                case ISqlServerBinaryTableNode binaryTableNode:
                    return new ImmutableSqlServerBinaryTableNode(
                        binaryTableNode.Parent,
                        binaryTableNode.Left,
                        binaryTableNode.BinaryTableType,
                        binaryTableNode.Right,
                        binaryTableNode.UserData);
            }

            return base.ReassembleCommonTableNode(node);
        }
    }
}
