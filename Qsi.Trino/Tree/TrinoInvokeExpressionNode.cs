using System.Collections.Generic;
using System.Linq;
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

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                IEnumerable<IQsiTreeNode> enumerable = base.Children;

                if (!OrderBy.IsEmpty)
                    enumerable = enumerable.Append(OrderBy.Value);

                if (!Filter.IsEmpty)
                    enumerable = enumerable.Append(Filter.Value);

                if (!Over.IsEmpty)
                    enumerable = enumerable.Append(Over.Value);

                return enumerable;
            }
        }

        public TrinoInvokeExpressionNode()
        {
            OrderBy = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Filter = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Over = new QsiTreeNodeProperty<TrinoWindowExpressionNode>(this);
        }
    }
}
