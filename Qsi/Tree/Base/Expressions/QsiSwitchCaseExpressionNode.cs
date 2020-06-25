namespace Qsi.Tree.Base
{
    public sealed class QsiSwitchCaseExpressionNode : QsiExpressionNode, IQsiSwitchCaseExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Consequent { get; }

        #region Explicit
        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Condition => Condition.GetValue();

        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Consequent => Consequent.GetValue();
        #endregion

        public QsiSwitchCaseExpressionNode()
        {
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Consequent = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
