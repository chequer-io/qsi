namespace Qsi.Tree.Base
{
    public sealed class QsiLogicalExpressionNode : QsiExpressionNode, IQsiLogicalExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

        #region Explicit
        IQsiExpressionNode IQsiLogicalExpressionNode.Left => Left.GetValue();

        IQsiExpressionNode IQsiLogicalExpressionNode.Right => Right.GetValue();
        #endregion

        public QsiLogicalExpressionNode()
        {
            Left = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Right = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
