using System.Collections.Generic;
using System.Linq.Expressions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Nodes
{
    public class AthenaStatementExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiTreeNode> Expression { get; }
        
        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public AthenaStatementExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiTreeNode>(this);
        }
    }
}
