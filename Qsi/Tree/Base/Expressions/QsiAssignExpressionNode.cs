namespace Qsi.Tree.Base
{
    public sealed class QsiAssignExpressionNode : QsiExpressionNode, IQsiAssignExpressionNode
    {
        public QsiVariableAccessExpressionNode Variable { get; set; }

        public string Operator { get; set; }

        public QsiExpressionNode Value { get; set; }

        #region Explicit
        IQsiVariableAccessExpressionNode IQsiAssignExpressionNode.Variable => Variable;

        IQsiExpressionNode IQsiAssignExpressionNode.Value => Value;
        #endregion
    }
}
