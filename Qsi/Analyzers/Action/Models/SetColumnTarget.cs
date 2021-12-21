using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Action.Models
{
    public sealed class SetColumnTarget : ColumnTarget
    {
        public IQsiExpressionNode ValueNode { get; }

        public SetColumnTarget(
            int declaredOrder,
            QsiQualifiedIdentifier declaredName,
            QsiTableColumn targetColumn,
            QsiTableColumn affectedReferenceColumn,
            IQsiExpressionNode valueNode)
            : base(declaredOrder, declaredName, targetColumn, affectedReferenceColumn)
        {
            ValueNode = valueNode;
        }
    }
}
