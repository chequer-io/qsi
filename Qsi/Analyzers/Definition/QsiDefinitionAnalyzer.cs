using System.Threading.Tasks;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Analyzers.Definition;

public class QsiDefinitionAnalyzer : QsiAnalyzerBase
{
    public QsiDefinitionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
    {
        return tree is IQsiDefinitionNode;
    }

    protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        switch (context.Tree)
        {
            case IQsiViewDefinitionNode viewDefinition:
                return await ExecuteViewDefinition(context, viewDefinition);

            default:
                throw TreeHelper.NotSupportedTree(context.Tree);
        }
    }

    private async ValueTask<IQsiAnalysisResult[]> ExecuteViewDefinition(IAnalyzerContext context, IQsiViewDefinitionNode viewDefinition)
    {
        using var tableContext = new TableCompileContext(context);

        var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
        var tableStructure = await tableAnalyzer.BuildViewDefinitionStructure(tableContext, viewDefinition);

        var result = new QsiTableDefinitionResult(viewDefinition.Identifier, tableStructure);

        return result.ToSingleArray();
    }
}
