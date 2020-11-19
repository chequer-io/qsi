using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiGroupingExpressionNode : QsiExpressionNode, IQsiGroupingExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Items { get; }

        public override IEnumerable<IQsiTreeNode> Children => Items;

        #region Explicit
        IQsiExpressionNode[] IQsiGroupingExpressionNode.Items => Items.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiGroupingExpressionNode()
        {
            Items = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
