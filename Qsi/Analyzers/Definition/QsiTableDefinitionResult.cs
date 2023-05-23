using Qsi.Data;

namespace Qsi.Analyzers.Definition;

public sealed class QsiTableDefinitionResult : IQsiAnalysisResult
{
    public QsiQualifiedIdentifier Name { get; }

    public QsiTableStructure Table { get; }

    public QsiSensitiveDataCollection SensitiveDataCollection => QsiSensitiveDataCollection.Empty;

    public QsiTableDefinitionResult(QsiQualifiedIdentifier name, QsiTableStructure table)
    {
        Name = name;
        Table = table;
    }
}
