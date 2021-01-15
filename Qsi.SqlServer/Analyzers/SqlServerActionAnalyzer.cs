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
                case SqlServerAlterUserActionNode alterUserActionNode:
                    return await ExecuteAlterUserAction(context, alterUserActionNode);
            }

            return await base.OnExecute(context);
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
