using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableMultipleExpressionNode : IQsiMultipleExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode[] Elements { get; }

        public IEnumerable<IQsiTreeNode> Children => Elements;

        public ImmutableMultipleExpressionNode(IQsiTreeNode parent, IQsiExpressionNode[] elements) : this()
        {
            Parent = parent;
            Elements = elements;
        }
    }
}
