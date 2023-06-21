using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaOrderByExpressionNode : QsiOrderExpressionNode
{
    public QsiTreeNodeProperty<HanaCollateExpressionNode> Collate { get; }

    public HanaOrderByNullBehavior? NullBehavior { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var child in base.Children)
                yield return child;

            if (!Collate.IsEmpty)
                yield return Collate.Value;
        }
    }

    public HanaOrderByExpressionNode()
    {
        Collate = new QsiTreeNodeProperty<HanaCollateExpressionNode>(this);
    }
}