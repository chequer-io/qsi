using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiSequentialColumnNode : QsiColumnNode, IQsiSequentialColumnNode
    {
        public int Ordinal { get; set; } = -1;

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public QsiSequentialColumnType ColumnType { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Alias);

        #region Explicit
        IQsiAliasNode IQsiSequentialColumnNode.Alias => Alias.Value;
        #endregion

        public QsiSequentialColumnNode()
        {
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
