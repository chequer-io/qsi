using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiGroupingExpressionNode : QsiExpressionNode, IQsiGroupingExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Items { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Having { get; }

        public override IEnumerable<IQsiTreeNode> Children => Items;

        #region Explicit
        IQsiExpressionNode[] IQsiGroupingExpressionNode.Items => Items.Cast<IQsiExpressionNode>().ToArray();

        IQsiExpressionNode IQsiGroupingExpressionNode.Having => Having.Value;
        #endregion

        public QsiGroupingExpressionNode()
        {
            Items = new QsiTreeNodeList<QsiExpressionNode>(this);
            Having = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
