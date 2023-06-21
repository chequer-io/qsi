using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Trino.Tree;

public class TrinoMergeActionNode : QsiActionNode
{
    public QsiTreeNodeList<QsiActionNode> ActionNodes { get; }

    public override IEnumerable<IQsiTreeNode> Children => ActionNodes;

    public TrinoMergeActionNode()
    {
        ActionNodes = new QsiTreeNodeList<QsiActionNode>(this);
    }
}