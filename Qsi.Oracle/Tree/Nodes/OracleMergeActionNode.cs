using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Oracle.Tree;

public class OracleMergeActionNode : QsiActionNode
{
    public QsiTreeNodeList<QsiActionNode> ActionNodes { get; }

    public string Hint { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => ActionNodes;

    public OracleMergeActionNode()
    {
        ActionNodes = new QsiTreeNodeList<QsiActionNode>(this);
    }
}