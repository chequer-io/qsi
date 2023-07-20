using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableUnaryExpressionNode : IQsiUnaryExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public string Operator { get; }

    public IQsiExpressionNode Expression { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    public ImmutableUnaryExpressionNode(IQsiTreeNode parent, string @operator, IQsiExpressionNode expression, IUserDataHolder userData)
    {
        Parent = parent;
        Operator = @operator;
        Expression = expression;
        UserData = userData;
    }
}