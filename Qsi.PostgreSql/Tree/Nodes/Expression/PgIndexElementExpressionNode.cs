using System.Collections.Generic;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgIndexElementExpressionNode : QsiExpressionNode
{
    public QsiIdentifier? Name { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiIdentifier? IndexColumnName { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> Collation { get; }

    public QsiTreeNodeList<QsiExpressionNode?> OpClass { get; }

    public QsiTreeNodeList<QsiExpressionNode?> OpClassOptions { get; }

    public SortByDir Ordering { get; set; }

    public SortByNulls NullsOrdering { get; set; }

    public PgIndexElementExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Collation = new QsiTreeNodeList<QsiExpressionNode?>(this);
        OpClass = new QsiTreeNodeList<QsiExpressionNode?>(this);
        OpClassOptions = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children =>
        TreeHelper.YieldChildren(Expression)
            .ConcatWhereNotNull(Collation)
            .ConcatWhereNotNull(OpClass)
            .ConcatWhereNotNull(OpClassOptions);
}
