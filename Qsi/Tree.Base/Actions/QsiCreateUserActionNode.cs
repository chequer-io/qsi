using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiCreateUserActionNode : QsiActionNode, IQsiCreateUserActionNode
{
    public QsiDataConflictBehavior ConflictBehavior { get; set; }

    public QsiTreeNodeList<QsiUserNode> Users { get; }

    IQsiUserNode[] IQsiCreateUserActionNode.Users => Users.Cast<IQsiUserNode>().ToArray();

    public override IEnumerable<IQsiTreeNode> Children => Users;

    public QsiCreateUserActionNode()
    {
        Users = new QsiTreeNodeList<QsiUserNode>(this);
    }
}
