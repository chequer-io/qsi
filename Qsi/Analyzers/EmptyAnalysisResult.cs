using Qsi.Data;

namespace Qsi.Analyzers
{
    public readonly struct EmptyAnalysisResult : IQsiAnalysisResult
    {
        QsiSensitiveDataHolder IQsiAnalysisResult.SensitiveDataHolder => null;
    }
}
