using System.Collections.Generic;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableParametersExpressionNode : IQsiParametersExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode[] Expressions { get; }

        public IEnumerable<IQsiTreeNode> Children => Expressions;

        public ImmutableParametersExpressionNode(IQsiTreeNode parent, IQsiExpressionNode[] expressions)
        {
            Parent = parent;
            Expressions = expressions;
        }
    }
}
