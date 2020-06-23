namespace Qsi.Tree.Base
{
    public sealed class QsiDerivedColumnNode : QsiColumnNode, IQsiDerivedColumnNode
    {
        public QsiColumnNode Column { get; set; }

        public QsiExpressionNode Expression { get; set; }

        public QsiAliasNode Alias { get; set; }

        #region Explicit
        IQsiColumnNode IQsiDerivedColumnNode.Column => Column;

        IQsiExpressionNode IQsiDerivedColumnNode.Expression => Expression;

        IQsiAliasNode IQsiDerivedColumnNode.Alias => Alias;
        #endregion
    }
}
