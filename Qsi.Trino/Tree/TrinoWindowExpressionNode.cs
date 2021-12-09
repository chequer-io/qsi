using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoWindowExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeList<TrinoWindowItemNode> Items { get; }

        public override IEnumerable<IQsiTreeNode> Children => Items;

        public TrinoWindowExpressionNode()
        {
            Items = new QsiTreeNodeList<TrinoWindowItemNode>(this);
        }
    }

    public class TrinoWindowItemNode : QsiTreeNode
    {
        public QsiIdentifier Identifier { get; set; }

        public QsiIdentifier ExistingWindow { get; set; }

        public QsiTreeNodeProperty<QsiMultipleExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Windowing { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Partition, Order, Windowing);

        public TrinoWindowItemNode()
        {
            Partition = new QsiTreeNodeProperty<QsiMultipleExpressionNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Windowing = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}
