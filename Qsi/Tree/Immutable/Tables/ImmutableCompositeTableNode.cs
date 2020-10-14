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
            IUserDataHolder userData, 
            IQsiMultipleOrderExpressionNode orderExpression, 
            IQsiLimitExpressionNode limitExpression)
        {
            Parent = parent;
            Sources = sources;
            UserData = userData;
            OrderExpression = orderExpression;
            LimitExpression = limitExpression;
        }
    }
}
