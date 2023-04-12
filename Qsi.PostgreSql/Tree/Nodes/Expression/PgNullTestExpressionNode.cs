using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

// <target> IS [NOT] NULL
public class PgNullTestExpressionNode : QsiExpressionNode
{
    public bool IsNot { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Target { get; }

    public PgNullTestExpressionNode()
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
