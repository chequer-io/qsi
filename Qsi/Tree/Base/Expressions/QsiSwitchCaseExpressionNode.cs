namespace Qsi.Tree.Base
{
    public sealed class QsiSwitchCaseExpressionNode : QsiExpressionNode, IQsiSwitchCaseExpressionNode
    {
        public QsiExpressionNode Condition { get; set; }

        public QsiExpressionNode Return { get; set; }

        #region Explicit
        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Condition => Condition;

        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Return => Return;
        #endregion
    }
}
