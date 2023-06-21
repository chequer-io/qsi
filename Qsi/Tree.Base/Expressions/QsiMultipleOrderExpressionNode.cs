using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree;

public class QsiMultipleOrderExpressionNode : QsiExpressionNode, IQsiMultipleOrderExpressionNode
{
    public QsiTreeNodeList<QsiOrderExpressionNode> Orders { get; }

    public override IEnumerable<IQsiTreeNode> Children => Orders;

    #region Explicit
    IQsiOrderExpressionNode[] IQsiMultipleOrderExpressionNode.Orders => Orders.Cast<IQsiOrderExpressionNode>().ToArray();
    #endregion

    public QsiMultipleOrderExpressionNode()
    {
        Orders = new QsiTreeNodeList<QsiOrderExpressionNode>(this);
    }
}