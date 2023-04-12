using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgGroupingSetExpressionNode : QsiExpressionNode
{
    public GroupingSetKind Kind { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> Expressions { get; }

    public PgGroupingSetExpressionNode()
    {
        Expressions = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => Expressions.Where(e => e is { }).Cast<IQsiTreeNode>();
}
