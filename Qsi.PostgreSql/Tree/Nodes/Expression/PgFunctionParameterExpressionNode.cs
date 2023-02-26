using System.Collections.Generic;
using PgQuery;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgFunctionParameterExpressionNode : QsiExpressionNode
{
    public QsiIdentifier Name { get; set; } = QsiIdentifier.Empty;

    public QsiTreeNodeProperty<PgTypeExpressionNode> TypeName { get; }

    public FunctionParameterMode Mode { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> DefinitionExpression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(TypeName, DefinitionExpression);

    public PgFunctionParameterExpressionNode()
    {
        TypeName = new QsiTreeNodeProperty<PgTypeExpressionNode>(this);
        DefinitionExpression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
