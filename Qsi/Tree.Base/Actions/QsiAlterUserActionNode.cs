using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiAlterUserActionNode : QsiActionNode, IQsiAlterUserActionNode
{
    public QsiDataConflictBehavior ConflictBehavior { get; set; }

    public QsiTreeNodeList<QsiUserNode> Users { get; }

    IQsiUserNode[] IQsiAlterUserActionNode.Users => Users.Cast<IQsiUserNode>().ToArray();

    public override IEnumerable<IQsiTreeNode> Children => Users;

    public QsiAlterUserActionNode()
    {
        Users = new QsiTreeNodeList<QsiUserNode>(this);
    }
}
