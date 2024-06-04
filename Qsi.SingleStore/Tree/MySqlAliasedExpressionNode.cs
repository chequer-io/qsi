using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public class SingleStoreAliasedExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Expression.IsEmpty)
                yield return Expression.Value;

            if (!Alias.IsEmpty)
                yield return Alias.Value;
        }
    }

    public SingleStoreAliasedExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
    }
}
