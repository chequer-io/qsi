using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiColumnsDeclarationNode : QsiTreeNode, IQsiColumnsDeclarationNode
    {
        public QsiTreeNodeList<QsiColumnNode> Columns { get; }

        #region Explicit
        IQsiColumnNode[] IQsiColumnsDeclarationNode.Columns => Columns.Cast<IQsiColumnNode>().ToArray();
        #endregion

        public QsiColumnsDeclarationNode()
        {
            Columns = new QsiTreeNodeList<QsiColumnNode>(this);
        }
    }
}
