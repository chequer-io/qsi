using Qsi.Data;

namespace Qsi.Analyzers.Action.Models;

//              +---+---+
// Source       | A | D |
//              +-┬-+-┬-+ 
//                │   └───────┐
//              +-┴-+---+---+-┴-+
// Destination  | A | B | C | D |
//              +---+---+---+---+
public sealed class DataManipulationTargetDataPivot
{
    public ColumnTarget DeclaredColumnTarget { get; }

    public int SourceOrder { get; }

    public QsiTableColumn SourceColumn { get; }

    public int DestinationOrder { get; }

    public QsiTableColumn DestinationColumn { get; }

    public DataManipulationTargetDataPivot(
        ColumnTarget declaredColumnTarget,
        int destinationOrder, QsiTableColumn destinationColumn,
        int sourceOrder, QsiTableColumn sourceColumn)
    {
        DeclaredColumnTarget = declaredColumnTarget;
        DestinationOrder = destinationOrder;
        DestinationColumn = destinationColumn;
        SourceOrder = sourceOrder;
        SourceColumn = sourceColumn;
    }
}