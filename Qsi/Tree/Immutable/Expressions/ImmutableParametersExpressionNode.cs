using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableParametersExpressionNode : IQsiParametersExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiExpressionNode[] Expressions { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Expressions;

    public ImmutableParametersExpressionNode(IQsiTreeNode parent, IQsiExpressionNode[] expressions, IUserDataHolder userData)
    {
        Parent = parent;
        Expressions = expressions;
        UserData = userData;
    }
}