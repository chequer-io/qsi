using Qsi.Data;

namespace Qsi.Analyzers.Action.Models;

public class ColumnTarget
{
    public int DeclaredOrder { get; }

    public QsiQualifiedIdentifier DeclaredName { get; }

    public QsiTableColumn SourceColumn { get; }

    public QsiTableColumn AffectedColumn { get; }

    public ColumnTarget(
        int declaredOrder,
        QsiQualifiedIdentifier declaredName,
        QsiTableColumn sourceColumn,
        QsiTableColumn affectedColumn)
    {
        DeclaredOrder = declaredOrder;
        DeclaredName = declaredName;
        SourceColumn = sourceColumn;
        AffectedColumn = affectedColumn;
    }
}