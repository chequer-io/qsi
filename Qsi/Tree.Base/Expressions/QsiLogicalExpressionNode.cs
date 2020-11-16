using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiLogicalExpressionNode : QsiExpressionNode, IQsiLogicalExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Left, Right);

        #region Explicit
        IQsiExpressionNode IQsiLogicalExpressionNode.Left => Left.Value;

        IQsiExpressionNode IQsiLogicalExpressionNode.Right => Right.Value;
        #endregion

        public QsiLogicalExpressionNode()
        {
            Left = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Right = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
