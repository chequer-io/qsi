using System.Collections.Generic;
using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

// Arg IS [NOT] {TRUE|FALSE}
public class PgBooleanTestExpressionNode : QsiExpressionNode
{
    public BoolTestType BoolTestType { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Target { get; }

    public PgBooleanTestExpressionNode()
    {
        Target = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Target.IsEmpty)
                yield return Target.Value;
        }
    }
}
