using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiLogicalExpressionNode : QsiExpressionNode, IQsiLogicalExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Left.IsEmpty)
                    yield return Left.Value;

                if (!Right.IsEmpty)
                    yield return Right.Value;
            }
        }

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
