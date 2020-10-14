using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class TableDataColumnPivot
    {
        public QsiTableColumn TargetColumn { get; }

        public QsiTableColumn DeclaredColumn { get; }

        public int TargetOrder { get; }

        public int DeclaredOrder { get; }

        public TableDataColumnPivot(
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
