using Qsi.Analyzers;

namespace Qsi.Data;

public class QsiGrantUserActionResult : IQsiAnalysisResult
{
    public string[] Roles { get; set; }

    public QsiUserInfo[] TargetUsers { get; set; }
}
