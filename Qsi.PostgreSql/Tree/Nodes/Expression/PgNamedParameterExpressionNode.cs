using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgNamedParameterExpressionNode : QsiExpressionNode
{
    public string? Name { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public PgNamedParameterExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);
}
