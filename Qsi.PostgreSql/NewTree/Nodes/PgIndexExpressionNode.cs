using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgIndexExpressionNode : QsiExpressionNode
{
    // [1]
    public QsiTreeNodeProperty<QsiExpressionNode> Index { get; }

    // [1..3]
    public QsiTreeNodeProperty<QsiExpressionNode> IndexEnd { get; }

    public PgIndexExpressionNode()
    {
        Index = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        IndexEnd = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Index, IndexEnd);
}
