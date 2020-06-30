namespace Qsi.Tree.Base
{
    public sealed class QsiSequentialColumnNode : QsiColumnNode, IQsiSequentialColumnNode
    {
        public int Ordinal { get; set; } = -1;

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        #region Explicit
        IQsiAliasNode IQsiSequentialColumnNode.Alias => Alias.GetValue();
        #endregion

        public QsiSequentialColumnNode()
        {
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
