using Qsi.Data;

namespace Qsi.Analyzers.Action
{
    public sealed class QsiActionAnalysisResult : IQsiAnalysisResult
    {
        public QsiAction Action { get; }

        public QsiActionAnalysisResult(QsiAction action)
        {
            Action = action;
        }
    }
}
