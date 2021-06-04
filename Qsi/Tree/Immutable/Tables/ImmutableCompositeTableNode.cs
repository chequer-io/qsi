using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableCompositeTableNode : IQsiCompositeTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode[] Sources { get; }

        public IQsiMultipleOrderExpressionNode Order { get; }

        public IQsiLimitExpressionNode Limit { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Sources;

        public ImmutableCompositeTableNode(
            IQsiTreeNode parent,
            IQsiTableNode[] sources,
            IQsiMultipleOrderExpressionNode orderExpression,
            IQsiLimitExpressionNode limitExpression,
            IUserDataHolder userData)
        {
            Parent = parent;
            Sources = sources;
            Order = orderExpression;
            Limit = limitExpression;
            UserData = userData;
        }
    }
}
