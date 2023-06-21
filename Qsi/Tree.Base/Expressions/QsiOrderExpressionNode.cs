using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiOrderExpressionNode : QsiExpressionNode, IQsiOrderExpressionNode
{
    public QsiSortOrder Order { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    #region Explicit
    IQsiExpressionNode IQsiOrderExpressionNode.Expression => Expression.Value;
    #endregion

    public QsiOrderExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}