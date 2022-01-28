using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes;

public class AthenaLateralTableNode : QsiTableNode
{
    public AthenaLateralTableNode()
    {
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
    }

    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Source);
}
