namespace Qsi.Tree.Base
{
    public sealed class QsiDerivedColumnNode : QsiColumnNode, IQsiDerivedColumnNode
    {
        public QsiTreeNodeProperty<QsiColumnNode> Column { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        #region Explicit
        IQsiColumnNode IQsiDerivedColumnNode.Column => Column.GetValue();

        IQsiExpressionNode IQsiDerivedColumnNode.Expression => Expression.GetValue();

        IQsiAliasNode IQsiDerivedColumnNode.Alias => Alias.GetValue();
        #endregion

        public QsiDerivedColumnNode()
        {
            Column = new QsiTreeNodeProperty<QsiColumnNode>(this);
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
