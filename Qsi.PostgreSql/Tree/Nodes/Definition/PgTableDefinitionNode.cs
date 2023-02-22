using System.Collections.Generic;
using PgQuery;
using Qsi.PostgreSql.Data;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgTableDefinitionNode : QsiTableDefinitionNode
{
    public bool IsCreateTableAs { get; set; }

    public Relpersistence Relpersistence { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> TableElts { get; }

    public QsiTreeNodeList<QsiExpressionNode?> InheritRelations { get; }

    public QsiTreeNodeList<QsiExpressionNode?> Constraints { get; }

    public QsiTreeNodeList<QsiExpressionNode?> Options { get; }

    public QsiTreeNodeProperty<PgTypeExpressionNode> OfType { get; }

    public OnCommitAction OnCommit { get; set; }

    public string? TablespaceName { get; set; }

    public string? AccessMethod { get; set; }

    public bool IsInherit { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var child in TreeHelper.YieldChildren(Columns, ColumnSource, DataSource, OfType))
                yield return child;

            foreach (var child in TableElts)
            {
                if (child is { })
                    yield return child;
            }

            foreach (var child in InheritRelations)
            {
                if (child is { })
                    yield return child;
            }

            foreach (var child in Constraints)
            {
                if (child is { })
                    yield return child;
            }

            foreach (var child in Options)
            {
                if (child is { })
                    yield return child;
            }
        }
    }

    public PgTableDefinitionNode()
    {
        TableElts = new QsiTreeNodeList<QsiExpressionNode?>(this);
        InheritRelations = new QsiTreeNodeList<QsiExpressionNode?>(this);
        Constraints = new QsiTreeNodeList<QsiExpressionNode?>(this);
        Options = new QsiTreeNodeList<QsiExpressionNode?>(this);
        OfType = new QsiTreeNodeProperty<PgTypeExpressionNode>(this);
    }
}
