using System.Linq;
using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataUpdateActionNode : IQsiDataUpdateActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableAccessNode Target { get; }

        public IQsiSetColumnExpressionNode[] SetValues { get; }

        public IQsiWhereExpressionNode WhereExpression { get; }

        public IQsiMultipleOrderExpressionNode OrderExpression { get; }

        public IQsiLimitExpressionNode LimitExpression { get; }

        public IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Target)
                .Concat(SetValues)
                .Concat(TreeHelper.YieldChildren(WhereExpression, OrderExpression, LimitExpression));

        public ImmutableDataUpdateActionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiTableAccessNode target,
            IQsiSetColumnExpressionNode[] setValues,
            IQsiWhereExpressionNode whereExpression,
            IQsiMultipleOrderExpressionNode orderExpression,
            IQsiLimitExpressionNode limitExpression)
        {
            Parent = parent;
            UserData = userData;
            Target = target;
            SetValues = setValues;
            WhereExpression = whereExpression;
            OrderExpression = orderExpression;
            LimitExpression = limitExpression;
        }
    }
}
