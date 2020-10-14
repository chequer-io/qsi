using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableInvokeExpressionNode : IQsiInvokeExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiFunctionAccessExpressionNode Member { get; }

        public IQsiParametersExpressionNode Parameters { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Member, Parameters);

        public ImmutableInvokeExpressionNode(
            IQsiTreeNode parent,
            IQsiFunctionAccessExpressionNode member,
            IQsiParametersExpressionNode parameters,
            IUserDataHolder userData)
        {
            Parent = parent;
            Member = member;
            Parameters = parameters;
            UserData = userData;
        }
    }
}
