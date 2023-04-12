using System.Collections.Generic;
using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgOnConflictNode : QsiTreeNode
{
    public OnConflictAction Action { get; set; }

    public QsiTreeNodeProperty<PgInferExpressionNode> Infer { get; }

    public QsiTreeNodeList<QsiExpressionNode?> TargetList { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Where { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Infer.IsEmpty)
                yield return Infer.Value;

            if (!Where.IsEmpty)
                yield return Where.Value;

            foreach (var target in TargetList)
            {
                if (target is { })
                    yield return target;
            }
        }
    }

    public PgOnConflictNode()
    {
        Infer = new QsiTreeNodeProperty<PgInferExpressionNode>(this);
        TargetList = new QsiTreeNodeList<QsiExpressionNode?>(this);
        Where = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
