using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgExpressionWrapNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiTreeNode> Item { get; }

    public PgExpressionWrapNode()
    {
        Item = new QsiTreeNodeProperty<QsiTreeNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Item);
}
