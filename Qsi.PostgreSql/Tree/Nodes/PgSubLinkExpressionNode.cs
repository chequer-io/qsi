using System.Collections.Generic;
using PgQuery;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgSubLinkExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiTreeNodeProperty<QsiTableNode> Table { get; }

    public QsiQualifiedIdentifier? OperatorName { get; set; }

    public SubLinkType SubLinkType { get; set; }

    public PgSubLinkExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Table = new QsiTreeNodeProperty<QsiTableNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Table, Expression);
}
