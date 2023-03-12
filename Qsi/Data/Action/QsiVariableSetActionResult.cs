using Qsi.Analyzers;

namespace Qsi.Data;

public class QsiVariableSetActionResult : IQsiAnalysisResult
{
    public QsiIdentifier Name { get; set; }

    public QsiSensitiveDataHolder SensitiveDataHolder { get; } = new();
}
