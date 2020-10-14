using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableLimitExpressionNode : IQsiLimitExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiExpressionNode Limit { get; }

        public IQsiExpressionNode Offset { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Limit, Offset);

        public ImmutableLimitExpressionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiExpressionNode limit,
            IQsiExpressionNode offset)
        {
            Parent = parent;
            UserData = userData;
            Limit = limit;
            Offset = offset;
        }
    }
}
