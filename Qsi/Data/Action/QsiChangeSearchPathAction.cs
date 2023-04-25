using Qsi.Analyzers;

namespace Qsi.Data
{
    public class QsiChangeSearchPathAction : IQsiAnalysisResult
    {
        public QsiQualifiedIdentifier[] Identifiers { get; }

        public QsiChangeSearchPathAction(QsiQualifiedIdentifier[] identifiers)
        {
            Identifiers = identifiers;
        }
    }
}
