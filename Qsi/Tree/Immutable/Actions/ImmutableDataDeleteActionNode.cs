using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataDeleteActionNode : IQsiDataDeleteActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableNode Target { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target);

        public ImmutableDataDeleteActionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiTableNode target,
            IQsiWhereExpressionNode whereExpression)
        {
            Parent = parent;
            UserData = userData;
            Target = target;
        }
    }
}
