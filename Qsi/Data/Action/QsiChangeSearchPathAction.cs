using Qsi.Analyzers;

namespace Qsi.Data
{
    public class QsiChangeSearchPathAction : IQsiAnalysisResult
    {
        public QsiQualifiedIdentifier[] Identifiers { get; }

        public QsiSensitiveDataHolder SensitiveDataHolder => null;

        public QsiChangeSearchPathAction(QsiQualifiedIdentifier[] identifiers)
        {
            Identifiers = identifiers;
        }
    }
}
