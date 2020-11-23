using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiColumnsDeclarationNode : QsiTreeNode, IQsiColumnsDeclarationNode
    {
        public int Count => Columns.Count;

        public QsiTreeNodeList<QsiColumnNode> Columns { get; }

        public bool IsEmpty => Columns.Count == 0;

        public override IEnumerable<IQsiTreeNode> Children => Columns;

        #region Explicit
        IQsiColumnNode[] IQsiColumnsDeclarationNode.Columns => Columns.Cast<IQsiColumnNode>().ToArray();
        #endregion

        public QsiColumnsDeclarationNode()
        {
            Columns = new QsiTreeNodeList<QsiColumnNode>(this);
        }

        public IEnumerator<IQsiColumnNode> GetEnumerator()
        {
            return Columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
