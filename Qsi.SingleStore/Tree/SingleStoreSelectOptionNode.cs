using System.Collections.Generic;
using Qsi.SingleStore.Data;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public sealed class SingleStoreSelectOptionNode : QsiTreeNode
{
    public SingleStoreSelectOption Option { get; set; }

    public QsiTreeNodeProperty<QsiLiteralExpressionNode> MaxStatementTime { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!MaxStatementTime.IsEmpty)
                yield return MaxStatementTime.Value;
        }
    }

    public SingleStoreSelectOptionNode()
    {
        MaxStatementTime = new QsiTreeNodeProperty<QsiLiteralExpressionNode>(this);
    }
}
