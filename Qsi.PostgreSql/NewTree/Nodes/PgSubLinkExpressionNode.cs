using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgSubLinkExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiTreeNodeProperty<QsiTableNode> Table { get; }

    public QsiQualifiedIdentifier? OperatorName { get; set; }

    public string? SubLinkType { get; set; }

    public PgSubLinkExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Table = new QsiTreeNodeProperty<QsiTableNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Table, Expression);
}
