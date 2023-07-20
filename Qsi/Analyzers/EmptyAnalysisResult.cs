using Qsi.Data;

namespace Qsi.Analyzers;

public readonly struct EmptyAnalysisResult : IQsiAnalysisResult
{
    QsiSensitiveDataCollection IQsiAnalysisResult.SensitiveDataCollection => QsiSensitiveDataCollection.Empty;
}