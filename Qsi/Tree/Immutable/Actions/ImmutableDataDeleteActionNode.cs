using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataDeleteActionNode : IQsiDataDeleteActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableAccessNode Target { get; }

        public IQsiWhereExpressionNode WhereExpression { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target, WhereExpression);

        public ImmutableDataDeleteActionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiTableAccessNode target,
            IQsiWhereExpressionNode whereExpression)
        {
            Parent = parent;
            UserData = userData;
            Target = target;
            WhereExpression = whereExpression;
        }
    }
}
