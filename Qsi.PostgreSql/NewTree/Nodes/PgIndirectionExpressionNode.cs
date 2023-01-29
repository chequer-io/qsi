using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgIndirectionExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiDerivedColumnNode> Target { get; }

    public QsiTreeNodeList<QsiExpressionNode> Indirections { get; }

    public PgIndirectionExpressionNode()
    {
        Target = new QsiTreeNodeProperty<QsiDerivedColumnNode>(this);
        Indirections = new QsiTreeNodeList<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target, Indirections);
}
