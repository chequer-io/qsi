using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes;

public class AthenaUnloadActionNode : QsiActionNode
{
    public AthenaUnloadActionNode()
    {
        Query = new QsiTreeNodeProperty<QsiTableNode>(this);
    }

    public string Location { get; set; }

    public QsiTreeNodeProperty<QsiTableNode> Query { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);
}
