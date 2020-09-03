using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableDirectivesNode : QsiTableNode, IQsiTableDirectivesNode
    {
        public QsiTreeNodeList<QsiTableNode> Tables { get; }

        public bool IsRecursive { get; set; }

        #region Explicit
        IQsiTableNode[] IQsiTableDirectivesNode.Tables => Tables.Cast<IQsiTableNode>().ToArray();
        #endregion

        public QsiTableDirectivesNode()
        {
            Tables = new QsiTreeNodeList<QsiTableNode>(this);
        }
    }
}
