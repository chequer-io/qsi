using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree;

public sealed class TrinoLateralTableNode : QsiTableNode
{
    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Source);

    public TrinoLateralTableNode()
    {
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
    }
}