using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public class SingleStoreCollationExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiIdentifier Collate { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Expression.IsEmpty)
                yield return Expression.Value;
        }
    }

    public SingleStoreCollationExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
