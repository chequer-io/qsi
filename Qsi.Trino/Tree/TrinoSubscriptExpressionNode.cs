using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoSubscriptExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Index { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value, Index);

        public TrinoSubscriptExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Index = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
