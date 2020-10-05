using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public sealed class QsiCompositeTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeList<QsiTableNode> Sources { get; }

        public override IEnumerable<IQsiTreeNode> Children => Sources;

        #region Explicit
        IQsiTableNode[] IQsiCompositeTableNode.Sources => Sources.Cast<IQsiTableNode>().ToArray();
        #endregion

        public QsiCompositeTableNode()
        {
            Sources = new QsiTreeNodeList<QsiTableNode>(this);
        }
    }
}
