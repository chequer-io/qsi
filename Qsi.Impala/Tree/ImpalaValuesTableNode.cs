using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaValuesTableNode : QsiTableNode
    {
        public QsiTreeNodeList<QsiRowValueExpressionNode> Rows { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> Limit { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var row in Rows)
                    yield return row;

                if (!Order.IsEmpty)
                    yield return Order.Value;

                if (!Limit.IsEmpty)
                    yield return Limit.Value;
            }
        }

        public ImpalaValuesTableNode()
        {
            Rows = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Limit = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}
