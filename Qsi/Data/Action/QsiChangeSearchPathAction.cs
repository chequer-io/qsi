using Qsi.Analyzers;

namespace Qsi.Data;

public class QsiChangeSearchPathAction : IQsiAnalysisResult
{
    public QsiQualifiedIdentifier[] Identifiers { get; }

    public QsiSensitiveDataCollection SensitiveDataCollection => QsiSensitiveDataCollection.Empty;

    public QsiChangeSearchPathAction(QsiQualifiedIdentifier[] identifiers)
    {
        Identifiers = identifiers;
    }
}