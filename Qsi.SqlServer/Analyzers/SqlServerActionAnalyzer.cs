using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;

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
    }
}
