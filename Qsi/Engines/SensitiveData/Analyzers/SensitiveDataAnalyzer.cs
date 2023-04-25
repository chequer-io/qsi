using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.Engines.SensitiveData;

public abstract class SensitiveDataAnalyzer : QsiAnalyzerBase
{
    protected SensitiveDataAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    protected sealed override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        return new SensitiveDataAnalysisResult
        {
            SensitiveDataCollection = { await Analyze(context) }
        }.ToSingleArray();
    }

    protected abstract ValueTask<IEnumerable<QsiSensitiveData>> Analyze(IAnalyzerContext context);

    public sealed override bool CanExecute(QsiScript script, IQsiTreeNode tree) => true;
}
