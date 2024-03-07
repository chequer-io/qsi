using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Engines;
using Qsi.Tree;
using Qsi.Trino.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Analyzers;

public sealed class TrinoActionAnalyzer : QsiActionAnalyzer
{
    public TrinoActionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        switch (context.Tree)
        {
            case TrinoMergeActionNode mergeActionNode:
                return await ExecuteMergeAction(context, mergeActionNode);
        }

        return await base.OnExecute(context);
    }

    private async ValueTask<IQsiAnalysisResult[]> ExecuteMergeAction(IAnalyzerContext context, TrinoMergeActionNode mergeActionNode)
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
}