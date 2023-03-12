using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree;

public class QsiVariableSetActionNode : QsiActionNode, IQsiVariableSetActionNode
{
    public QsiTreeNodeList<QsiVariableSetItemNode> SetItems { get; }

    IQsiVariableSetItemNode[] IQsiVariableSetActionNode.SetItems => SetItems.Cast<IQsiVariableSetItemNode>().ToArray();

    public QsiVariableSetActionNode()
    {
        SetItems = new QsiTreeNodeList<QsiVariableSetItemNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => SetItems;
}
