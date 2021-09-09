using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoSubscriptExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Index { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public TrinoSubscriptExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Index = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
