namespace Qsi.Tree.Base
{
    public sealed class QsiTableExpressionNode : QsiExpressionNode, IQsiTableExpressionNode
    {
        public QsiTableNode Table { get; set; }

        #region Explicit
        IQsiTableNode IQsiTableExpressionNode.Table => Table;
        #endregion
    }
}
