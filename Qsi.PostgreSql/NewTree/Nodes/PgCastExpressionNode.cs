using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgCastExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Source { get; }

    public QsiTreeNodeProperty<QsiTypeExpressionNode> Type { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Source, Type);

    public PgCastExpressionNode()
    {
        Source = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Type = new QsiTreeNodeProperty<QsiTypeExpressionNode>(this);
    }
}
