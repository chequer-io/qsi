using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgCollateExpressionNode : QsiExpressionNode
{
    public QsiQualifiedIdentifier? Column { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public PgCollateExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Expression.IsEmpty)
                yield return Expression.Value;
        }
    }
}
