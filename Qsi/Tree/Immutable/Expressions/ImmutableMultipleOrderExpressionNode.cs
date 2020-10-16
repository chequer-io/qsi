using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableMultipleOrderExpressionNode : IQsiMultipleOrderExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiOrderExpressionNode[] Orders { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Orders;

        public ImmutableMultipleOrderExpressionNode(
            IQsiTreeNode parent,
            IQsiOrderExpressionNode[] orders,
            IUserDataHolder userData)
        {
            Parent = parent;
            Orders = orders;
            UserData = userData;
        }
    }
}
