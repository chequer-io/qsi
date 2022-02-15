using Qsi.Analyzers.Table;
using Qsi.Data;

namespace Qsi.Analyzers.Definition;

public sealed class QsiTableDefinitionResult : QsiTableResult
{
    public QsiQualifiedIdentifier Name { get; }

    public QsiTableDefinitionResult(QsiQualifiedIdentifier name, QsiTableStructure table) : base(table)
    {
        Name = name;
    }
}
