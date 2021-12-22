using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes
{
    public class AthenaWindowExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeList<AthenaWindowItemNode> Items { get; }

        public override IEnumerable<IQsiTreeNode> Children => Items;

        public AthenaWindowExpressionNode()
        {
            Items = new QsiTreeNodeList<AthenaWindowItemNode>(this);
        }
    }

    public class AthenaWindowItemNode : QsiTreeNode
    {
        public QsiTreeNodeProperty<QsiMultipleExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Windowing { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Partition, Order, Windowing);

        public AthenaWindowItemNode()
        {
            Partition = new QsiTreeNodeProperty<QsiMultipleExpressionNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Windowing = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}
