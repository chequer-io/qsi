namespace Qsi.Tree.Base
{
    public sealed class QsiTableExpressionNode : QsiExpressionNode, IQsiTableExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Table { get; }

        #region Explicit
        IQsiTableNode IQsiTableExpressionNode.Table => Table.GetValue();
        #endregion

        public QsiTableExpressionNode()
        {
            Table = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}
