using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableOrderExpressionNode : IQsiOrderExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public QsiSortOrder Order { get; }

    public IQsiExpressionNode Expression { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    public ImmutableOrderExpressionNode(
        IQsiTreeNode parent,
        QsiSortOrder order,
        IQsiExpressionNode expression,
        IUserDataHolder userData)
    {
        Parent = parent;
        Order = order;
        Expression = expression;
        UserData = userData;
    }
}