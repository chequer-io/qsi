using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableLimitExpressionNode : IQsiLimitExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiExpressionNode Limit { get; }

        public IQsiExpressionNode Offset { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Limit, Offset);

        public ImmutableLimitExpressionNode(
            IQsiTreeNode parent,
            IQsiExpressionNode limit,
            IQsiExpressionNode offset,
            IUserDataHolder userData)
        {
            Parent = parent;
            Limit = limit;
            Offset = offset;
            UserData = userData;
        }
    }
}
