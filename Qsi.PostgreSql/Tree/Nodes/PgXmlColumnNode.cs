using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgXmlColumnNode : QsiColumnNode
{
    public QsiIdentifier Name { get; set; } = QsiIdentifier.Empty;

    public QsiTreeNodeProperty<PgTypeExpressionNode> TypeName { get; }

    public bool ForOrdinality { get; set; }

    public bool IsNotNull { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> ColumnExpression { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> ColumnDefExpression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(TypeName, ColumnExpression, ColumnDefExpression);

    public PgXmlColumnNode()
    {
        TypeName = new QsiTreeNodeProperty<PgTypeExpressionNode>(this);
        ColumnExpression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        ColumnDefExpression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
