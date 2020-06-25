namespace Qsi.Tree.Base
{
    public sealed class QsiUnaryExpressionNode : QsiExpressionNode, IQsiUnaryExpressionNode
    {
        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        #region Explicit
        IQsiExpressionNode IQsiUnaryExpressionNode.Expression => Expression.GetValue();
        #endregion

        public QsiUnaryExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
