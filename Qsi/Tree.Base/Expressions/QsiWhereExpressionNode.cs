using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiWhereExpressionNode : QsiExpressionNode, IQsiWhereExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    #region Explicit
    IQsiExpressionNode IQsiWhereExpressionNode.Expression => Expression.Value;
    #endregion

    public QsiWhereExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}