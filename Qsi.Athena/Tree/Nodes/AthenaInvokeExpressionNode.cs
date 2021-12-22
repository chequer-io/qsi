using System.Collections.Generic;
using System.Linq;
using Qsi.Athena.Common;
using Qsi.Tree;

namespace Qsi.Athena.Tree.Nodes
{
    public class AthenaInvokeExpressionNode : QsiInvokeExpressionNode
    {
        public AthenaSetQuantifier? SetQuantifier { get; set; }

        public AthenaProcessingMode? ProcessingMode { get; set; }

        public AthenaNullTreatment? NullTreatment { get; set; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderBy { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Filter { get; }

        public QsiTreeNodeProperty<AthenaWindowExpressionNode> Over { get; }

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

        public AthenaInvokeExpressionNode()
        {
            OrderBy = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Filter = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Over = new QsiTreeNodeProperty<AthenaWindowExpressionNode>(this);
        }
    }
}
