using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlDerivedTableNode : QsiDerivedTableNode
    {
        public QsiTreeNodeList<MySqlLockingNode> Lockings { get; }

        public override IEnumerable<IQsiTreeNode> Children => base.Children.Concat(Lockings);

        public MySqlDerivedTableNode()
        {
            Lockings = new QsiTreeNodeList<MySqlLockingNode>(this);
        }
    }
}
