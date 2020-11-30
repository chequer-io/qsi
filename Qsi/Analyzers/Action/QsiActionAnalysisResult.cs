using Qsi.Data;

namespace Qsi.Analyzers.Action
{
    public class QsiActionAnalysisResult : IQsiAnalysisResult
    {
        public IQsiAction Action { get; }

        public QsiActionAnalysisResult(IQsiAction action)
        {
            Action = action;
        }
    }
}
