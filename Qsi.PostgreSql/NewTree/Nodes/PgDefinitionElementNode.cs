using System.Collections.Generic;
using PgQuery;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgDefinitionElementNode : QsiExpressionNode
{
    public string DefinitionName { get; set; } = string.Empty;

    public string DefinitionNamespace { get; set; } = string.Empty;

    public DefElemAction Action { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public PgDefinitionElementNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);
}
