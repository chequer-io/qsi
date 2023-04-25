using Qsi.Analyzers;
using Qsi.Data;

namespace Qsi.Engines.SensitiveData;

public class SensitiveDataAnalysisResult : IQsiAnalysisResult
{
    public QsiSensitiveDataCollection SensitiveDataCollection { get; } = new();
}
