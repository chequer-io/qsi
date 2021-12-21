using Qsi.Data;

namespace Qsi.Analyzers.Action.Models
{
    public class ColumnTarget
    {
        public int DeclaredOrder { get; }

        public QsiQualifiedIdentifier DeclaredName { get; }

        public QsiTableColumn TargetColumn { get; }

        public QsiTableColumn AffectedReferenceColumn { get; }

        public ColumnTarget(
            int declaredOrder,
            QsiQualifiedIdentifier declaredName,
            QsiTableColumn targetColumn,
            QsiTableColumn affectedReferenceColumn)
        {
            DeclaredOrder = declaredOrder;
            DeclaredName = declaredName;
            TargetColumn = targetColumn;
            AffectedReferenceColumn = affectedReferenceColumn;
        }
    }
}
