using System;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Athena.Tree.Nodes;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.Shared.Extensions;

namespace Qsi.Athena.Analyzers
{
    public class AthenaActionAnalyzer : QsiActionAnalyzer
    {
        public AthenaActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
        {
            switch (context.Tree)
            {
                case AthenaUnloadActionNode unloadAction:
                    return OnExecuteUnloadAction(context, unloadAction);
            }
            
            return base.OnExecute(context);
        }

        private async ValueTask<IQsiAnalysisResult[]> OnExecuteUnloadAction(IAnalyzerContext context, AthenaUnloadActionNode action)
        {
            var analyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);
            
            var table = await analyzer.BuildTableStructure(tableContext, action.Query.Value);
            var result = new AthenaUnloadTableResult(table);

            return result.ToSingleArray();
        }
    }
}
