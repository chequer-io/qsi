using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiBinaryExpressionNode : QsiExpressionNode, IQsiBinaryExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

    public string Operator { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

    public override IEnumerable<IQsiTreeNode> Children
        => TreeHelper.YieldChildren(Left, Right);

    #region Explicit
    IQsiExpressionNode IQsiBinaryExpressionNode.Left => Left.Value;

    IQsiExpressionNode IQsiBinaryExpressionNode.Right => Right.Value;
    #endregion

    public QsiBinaryExpressionNode()
    {
        Left = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Right = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}