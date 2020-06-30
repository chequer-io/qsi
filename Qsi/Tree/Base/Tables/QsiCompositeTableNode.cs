using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiCompositeTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeList<QsiTableNode> Sources { get; }

        #region Explicit
        IQsiTableNode[] IQsiCompositeTableNode.Sources => Sources.Cast<IQsiTableNode>().ToArray();
        #endregion

        public QsiCompositeTableNode()
        {
            Sources = new QsiTreeNodeList<QsiTableNode>(this);
        }
    }
}
