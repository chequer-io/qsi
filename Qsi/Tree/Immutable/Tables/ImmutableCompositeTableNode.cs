using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableCompositeTableNode : IQsiCompositeTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode[] Sources { get; }

        public IQsiMultipleOrderExpressionNode OrderExpression { get; }

        public IQsiLimitExpressionNode LimitExpression { get; }

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
            OrderExpression = orderExpression;
            LimitExpression = limitExpression;
            UserData = userData;
        }
    }
}
