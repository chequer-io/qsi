using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiAllColumnNode : QsiColumnNode, IQsiAllColumnNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Path { get; set; }

        public bool IncludeInvisibleColumns { get; set; }

        public QsiTreeNodeList<QsiSequentialColumnNode> SequentialColumns { get; }

        public override IEnumerable<IQsiTreeNode> Children => SequentialColumns;

        #region Explicit
        IQsiSequentialColumnNode[] IQsiAllColumnNode.SequentialColumns => SequentialColumns.Cast<IQsiSequentialColumnNode>().ToArray();
        #endregion

        public QsiAllColumnNode()
        {
            SequentialColumns = new QsiTreeNodeList<QsiSequentialColumnNode>(this);
        }
    }
}
