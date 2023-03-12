using Qsi.Analyzers;

namespace Qsi.Data;

public class QsiUserActionResult : IQsiAnalysisResult
{
    public QsiUserInfo[] UserInfos { get; set; }

    public QsiSensitiveDataHolder SensitiveDataHolder { get; set; }
}
