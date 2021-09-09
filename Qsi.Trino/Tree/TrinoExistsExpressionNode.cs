using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoExistsExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Query { get; } 

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public TrinoExistsExpressionNode()
        {
            Query = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}
