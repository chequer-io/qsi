namespace Qsi.Tree.Base
{
    public sealed class QsiAssignExpressionNode : QsiExpressionNode, IQsiAssignExpressionNode
    {
        public QsiTreeNodeProperty<QsiVariableAccessExpressionNode> Variable { get; }

        public string Operator { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        #region Explicit
        IQsiVariableAccessExpressionNode IQsiAssignExpressionNode.Variable => Variable.GetValue();

        IQsiExpressionNode IQsiAssignExpressionNode.Value => Value.GetValue();
        #endregion

        public QsiAssignExpressionNode()
        {
            Variable = new QsiTreeNodeProperty<QsiVariableAccessExpressionNode>(this);
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
