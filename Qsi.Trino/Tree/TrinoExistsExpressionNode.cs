using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoExistsExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Query { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);

        public TrinoExistsExpressionNode()
        {
            Query = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}
