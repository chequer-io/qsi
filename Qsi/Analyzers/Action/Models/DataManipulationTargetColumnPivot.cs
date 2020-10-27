using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class DataManipulationTargetColumnPivot
    {
        public QsiTableColumn TargetColumn { get; }

        public QsiTableColumn DeclaredColumn { get; }

        public int TargetOrder { get; }

        public int DeclaredOrder { get; }

        public DataManipulationTargetColumnPivot(
            int targetOrder, QsiTableColumn targetColumn,
            int declaredOrder, QsiTableColumn declaredColumn)
        {
            TargetOrder = targetOrder;
            TargetColumn = targetColumn;
            DeclaredOrder = declaredOrder;
            DeclaredColumn = declaredColumn;
        }
    }
}
