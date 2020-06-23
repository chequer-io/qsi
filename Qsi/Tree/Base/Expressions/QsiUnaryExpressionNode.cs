namespace Qsi.Tree.Base
{
    public sealed class QsiUnaryExpressionNode : QsiExpressionNode, IQsiUnaryExpressionNode
    {
        public string Operator { get; set; }

        public QsiExpressionNode Expression { get; set; }

        #region Explicit
        IQsiExpressionNode IQsiUnaryExpressionNode.Expression => Expression;
        #endregion
    }
}
