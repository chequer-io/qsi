using System.Collections.Generic;
using Qsi.Data;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgColumnDefinitionNode : QsiTreeNode, IQsiDefinitionNode
{
    public QsiIdentifier Name { get; set; } = QsiIdentifier.Empty;

    public QsiTreeNodeProperty<QsiTypeExpressionNode> TypeName { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> RawDefault { get; }

    public QsiTreeNodeProperty<PgCollateExpressionNode> CollClause { get; }

    public QsiTreeNodeList<QsiTreeNode> Constraints { get; }

    public QsiTreeNodeList<QsiTreeNode> FdwOptions { get; }

    public override IEnumerable<IQsiTreeNode> Children =>
        TreeHelper.YieldChildren(TypeName, RawDefault, CollClause)
            .ConcatWhereNotNull(Constraints)
            .ConcatWhereNotNull(FdwOptions);

    public PgColumnDefinitionNode()
    {
        TypeName = new QsiTreeNodeProperty<QsiTypeExpressionNode>(this);
        RawDefault = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        CollClause = new QsiTreeNodeProperty<PgCollateExpressionNode>(this);
        Constraints = new QsiTreeNodeList<QsiTreeNode>(this);
        FdwOptions = new QsiTreeNodeList<QsiTreeNode>(this);
    }
}
