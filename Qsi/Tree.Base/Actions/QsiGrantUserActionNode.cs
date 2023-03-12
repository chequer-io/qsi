using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree;

public class QsiGrantUserActionNode : QsiActionNode, IQsiGrantUserActionNode
{
    public string[] Roles { get; set; }

    public bool AllPrivileges { get; set; }

    public QsiTreeNodeList<QsiUserNode> Users { get; }

    IQsiUserNode[] IQsiGrantUserActionNode.Users => Users.Cast<IQsiUserNode>().ToArray();

    public QsiGrantUserActionNode()
    {
        Users = new QsiTreeNodeList<QsiUserNode>(this);
    }

    public override IEnumerable<IQsiTreeNode> Children => Users;
}
