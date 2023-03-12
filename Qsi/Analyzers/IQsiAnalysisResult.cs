using Qsi.Data;

namespace Qsi.Analyzers
{
    public interface IQsiAnalysisResult
    {
        QsiSensitiveDataHolder SensitiveDataHolder { get; }
    }
}
