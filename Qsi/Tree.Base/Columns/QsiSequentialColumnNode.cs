using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiSequentialColumnNode : QsiColumnNode, IQsiSequentialColumnNode
    {
        public int Ordinal { get; set; } = -1;

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Alias);

        #region Explicit
        IQsiAliasNode IQsiSequentialColumnNode.Alias => Alias.Value;
        #endregion

        public QsiSequentialColumnNode()
        {
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
