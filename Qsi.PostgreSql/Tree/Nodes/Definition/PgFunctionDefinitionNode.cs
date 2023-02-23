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

    // TODO: To Parameter Node type
    public QsiTreeNodeList<QsiTreeNode> Parameters { get; }

    public QsiTreeNodeProperty<PgTypeExpressionNode> ReturnType { get; }

    // TODO: To Options Node type
    public QsiTreeNodeList<QsiTreeNode> Options { get; }

    public QsiTreeNodeProperty<QsiTreeNode> SqlBody { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(ReturnType);

    public PgFunctionDefinitionNode(QsiQualifiedIdentifier name)
    {
        Name = name;
        Parameters = new QsiTreeNodeList<QsiTreeNode>(this);
        ReturnType = new QsiTreeNodeProperty<PgTypeExpressionNode>(this);
        Options = new QsiTreeNodeList<QsiTreeNode>(this);
        SqlBody = new QsiTreeNodeProperty<QsiTreeNode>(this);
    }
}
