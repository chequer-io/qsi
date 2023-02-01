using System.Collections.Generic;
using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgGroupingSetExpressionNode : QsiExpressionNode
{
    public GroupingSetKind Kind { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> Expressions { get; }

    public PgGroupingSetExpressionNode()
    {
        Expressions = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => Expressions;
}
