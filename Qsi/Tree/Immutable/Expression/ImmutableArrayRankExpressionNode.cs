using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableArrayRankExpressionNode : IQsiArrayRankExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Array { get; }

        public IQsiExpressionNode Rank { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Array, Rank);

        public ImmutableArrayRankExpressionNode(IQsiTreeNode parent, IQsiExpressionNode array, IQsiExpressionNode rank)
        {
            Parent = parent;
            Array = array;
            Rank = rank;
        }
    }
}
