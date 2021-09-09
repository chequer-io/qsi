using Qsi.Tree;
using Qsi.Trino.Common;

namespace Qsi.Trino.Tree
{
    public class TrinoInvokeExpressionNode : QsiInvokeExpressionNode
    {
        public TrinoProcessingMode? ProcessingMode { get; set; }

        public TrinoNullTreatment? NullTreatment { get; set; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderBy { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Filter { get; }

        public QsiTreeNodeProperty<TrinoWindowExpressionNode> Over { get; }

        public TrinoInvokeExpressionNode()
        {
            OrderBy = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Filter = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Over = new QsiTreeNodeProperty<TrinoWindowExpressionNode>(this);
        }
    }
}
