using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgFunctionDefinitionNode : QsiTreeNode, IQsiDefinitionNode
{
    public QsiQualifiedIdentifier Name { get; set; }

    public bool Replace { get; set; }

    public bool IsProcedure { get; set; }

    public QsiTreeNodeList<PgFunctionParameterExpressionNode> Parameters { get; }

    public QsiTreeNodeProperty<PgTypeExpressionNode> ReturnType { get; }

    public QsiTreeNodeList<PgDefinitionElementNode> Options { get; }

    public QsiTreeNodeProperty<QsiTreeNode> SqlBody { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(ReturnType, Parameters);

    public PgFunctionDefinitionNode(QsiQualifiedIdentifier name)
    {
        Name = name;
        Parameters = new QsiTreeNodeList<PgFunctionParameterExpressionNode>(this);
        ReturnType = new QsiTreeNodeProperty<PgTypeExpressionNode>(this);
        Options = new QsiTreeNodeList<PgDefinitionElementNode>(this);
        SqlBody = new QsiTreeNodeProperty<QsiTreeNode>(this);
    }
}
