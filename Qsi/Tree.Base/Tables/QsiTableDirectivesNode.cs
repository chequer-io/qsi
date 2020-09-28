using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public sealed class QsiTableDirectivesNode : QsiTableNode, IQsiTableDirectivesNode
    {
        public QsiTreeNodeList<QsiDerivedTableNode> Tables { get; }

        public bool IsRecursive { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Tables;

        #region Explicit
        IQsiDerivedTableNode[] IQsiTableDirectivesNode.Tables => Tables.Cast<IQsiDerivedTableNode>().ToArray();
        #endregion

        public QsiTableDirectivesNode()
        {
            Tables = new QsiTreeNodeList<QsiDerivedTableNode>(this);
        }
    }
}
