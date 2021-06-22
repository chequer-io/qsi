using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Engines;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SqlServer.Analyzers
{
    public class SqlServerActionAnalyzer : QsiActionAnalyzer
    {
        public SqlServerActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
        {
            switch (context.Tree)
            {
                case SqlServerMergeActionNode mergeActionNode:
                    return await ExecuteMergeAction(context, mergeActionNode);

                case SqlServerAlterUserActionNode alterUserActionNode:
                    return new[] { await ExecuteAlterUserAction(context, alterUserActionNode) };
            }

            return await base.OnExecute(context);
        }

        protected async ValueTask<IQsiAnalysisResult[]> ExecuteMergeAction(IAnalyzerContext context, SqlServerMergeActionNode mergeActionNode)
        {
            var results = new List<IQsiAnalysisResult>();

            foreach (var actionNode in mergeActionNode.ActionNodes)
            {
                IQsiAnalysisResult[] result;

                switch (actionNode)
                {
                    case QsiDataInsertActionNode insertActionNode:
                    {
                        result = await ExecuteDataInsertAction(context, insertActionNode);
                        break;
                    }

                    case QsiDataDeleteActionNode deleteActionNode:
                    {
                        result = await ExecuteDataDeleteAction(context, deleteActionNode);
                        break;
                    }

                    case QsiDataUpdateActionNode updateActionNode:
                    {
                        result = await ExecuteDataUpdateAction(context, updateActionNode);
                        break;
                    }

                    default:
                        throw TreeHelper.NotSupportedTree(actionNode);
                }

                results.AddRange(result);
            }

            return results.ToArray();
        }

        protected ValueTask<IQsiAnalysisResult> ExecuteAlterUserAction(IAnalyzerContext context, SqlServerAlterUserActionNode alterUserActionNode)
        {
            var action = new SqlServerAlterUserAction
            {
                TargetUser = alterUserActionNode.TargetUser,
                NewUserName = alterUserActionNode.NewUserName,
                DefaultSchema = alterUserActionNode.DefaultSchema
            };

            return new ValueTask<IQsiAnalysisResult>(action);
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
