using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public class SqlServerMergeActionNode : QsiActionNode
    {
        public QsiTreeNodeList<QsiActionNode> ActionNodes { get; }

        public override IEnumerable<IQsiTreeNode> Children => ActionNodes;

        public SqlServerMergeActionNode()
        {
            ActionNodes = new QsiTreeNodeList<QsiActionNode>(this);
        }
    }
}
