using System.Collections.Generic;
using PgQuery;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDefinitionElementNode : QsiExpressionNode
{
    public string DefinitionName { get; set; } = string.Empty;

    public string DefinitionNamespace { get; set; } = string.Empty;

    public DefElemAction Action { get; set; }

    public QsiTreeNodeProperty<QsiTreeNode> Argument { get; }

    public PgDefinitionElementNode()
    {
        Argument = new QsiTreeNodeProperty<QsiTreeNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Argument);
}
