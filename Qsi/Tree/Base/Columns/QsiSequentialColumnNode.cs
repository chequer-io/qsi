using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiSequentialColumnNode : QsiColumnNode, IQsiSequentialColumnNode
    {
        public int Ordinal { get; set; } = -1;

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Alias.IsEmpty)
                    yield return Alias.Value;
            }
        }

        #region Explicit
        IQsiAliasNode IQsiSequentialColumnNode.Alias => Alias.Value;
        #endregion

        public QsiSequentialColumnNode()
        {
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
