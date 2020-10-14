using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableOrderExpressionNode : IQsiOrderExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public QsiSortOrder Order { get; }

        public IQsiExpressionNode Expression { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public ImmutableOrderExpressionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            QsiSortOrder order,
            IQsiExpressionNode expression)
        {
            Parent = parent;
            UserData = userData;
            Order = order;
            Expression = expression;
        }
    }
}
