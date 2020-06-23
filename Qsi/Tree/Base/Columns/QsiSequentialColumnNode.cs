namespace Qsi.Tree.Base
{
    public sealed class QsiSequentialColumnNode : QsiColumnNode, IQsiSequentialColumnNode
    {
        public int Ordinal { get; set; } = -1;

        public QsiAliasNode Alias { get; set; }

        #region Explicit
        IQsiAliasNode IQsiSequentialColumnNode.Alias => Alias;
        #endregion
    }
}
