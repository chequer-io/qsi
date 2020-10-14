using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableMultipleOrderExpressionNode : IQsiMultipleOrderExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiOrderExpressionNode[] Orders { get; }

        public IEnumerable<IQsiTreeNode> Children => Orders;

        public ImmutableMultipleOrderExpressionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiOrderExpressionNode[] orders)
        {
            Parent = parent;
            UserData = userData;
            Orders = orders;
        }
    }
}
