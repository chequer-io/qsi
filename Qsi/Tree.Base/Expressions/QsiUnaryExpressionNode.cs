using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiUnaryExpressionNode : QsiExpressionNode, IQsiUnaryExpressionNode
{
    public string Operator { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children
        => TreeHelper.YieldChildren(Expression);

    #region Explicit
    IQsiExpressionNode IQsiUnaryExpressionNode.Expression => Expression.Value;
    #endregion

    public QsiUnaryExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}