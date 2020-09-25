using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableLogicalExpressionNode : IQsiLogicalExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Left { get; }

        public string Operator { get; }

        public IQsiExpressionNode Right { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Left, Right);

        public ImmutableLogicalExpressionNode(
            IQsiTreeNode parent,
            IQsiExpressionNode left,
            string @operator,
            IQsiExpressionNode right)
        {
            Parent = parent;
            Left = left;
            Operator = @operator;
            Right = right;
        }
    }
}
