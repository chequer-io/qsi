namespace Qsi.Tree.Base
{
    public sealed class QsiLogicalExpressionNode : QsiExpressionNode, IQsiLogicalExpressionNode
    {
        public QsiExpressionNode Left { get; set; }

        public string Operator { get; set; }

        public QsiExpressionNode Right { get; set; }

        #region Explicit
        IQsiExpressionNode IQsiLogicalExpressionNode.Left => Left;

        IQsiExpressionNode IQsiLogicalExpressionNode.Right => Right;
        #endregion
    }
}
