using System.Collections.Generic;
using System.Linq.Expressions;
using Qsi.Tree;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes
{
    public class AthenaUnloadActionNode : QsiActionNode
    {
        public string Location { get; set; }
        
        public QsiTreeNodeProperty<QsiTableNode> Query { get; }
        
        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);

        public AthenaUnloadActionNode()
        {
            Query = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}
