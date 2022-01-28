using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes;

public class AthenaSubscriptExpressionNode : QsiExpressionNode
{
    public AthenaSubscriptExpressionNode()
    {
        Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Index = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Index { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value, Index);
}
