using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoWhenExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> When { get; }
        public QsiTreeNodeProperty<QsiExpressionNode> Then { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public TrinoWhenExpressionNode()
        {
            When = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Then = new QsiTreeNodeProperty<QsiExpressionNode>(this)l
        }
    }
}
