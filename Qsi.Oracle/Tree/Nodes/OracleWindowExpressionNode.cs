using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleWindowExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeList<OracleWindowItemNode> Items { get; }

    public override IEnumerable<IQsiTreeNode> Children => Items;

    public OracleWindowExpressionNode()
    {
        Items = new QsiTreeNodeList<OracleWindowItemNode>(this);
    }
}

public class OracleWindowItemNode : QsiTreeNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiQualifiedIdentifier ExistingWindow { get; set; }

    public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

    public QsiTreeNodeProperty<OracleMultipleOrderExpressionNode> Order { get; }

    public QsiTreeNodeProperty<OracleWindowingExpressionNode> Windowing { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Partition, Order, Windowing);

    public OracleWindowItemNode()
    {
        Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
        Order = new QsiTreeNodeProperty<OracleMultipleOrderExpressionNode>(this);
        Windowing = new QsiTreeNodeProperty<OracleWindowingExpressionNode>(this);
    }
}