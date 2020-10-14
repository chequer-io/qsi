using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiMultipleOrderExpressionNode : QsiExpressionNode, IQsiMultipleOrderExpressionNode
    {
        public QsiTreeNodeProperty<QsiOrderExpressionNode> Orders { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Orders);

        #region Explicit
        IQsiOrderExpressionNode IQsiMultipleOrderExpressionNode.Orders => Orders.Value;
        #endregion

        public QsiMultipleOrderExpressionNode()
        {
            Orders = new QsiTreeNodeProperty<QsiOrderExpressionNode>(this);
        }
    }
}
