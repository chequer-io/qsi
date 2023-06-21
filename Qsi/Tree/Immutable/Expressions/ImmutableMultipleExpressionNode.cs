using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableMultipleExpressionNode : IQsiMultipleExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiExpressionNode[] Elements { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Elements;

    public ImmutableMultipleExpressionNode(
        IQsiTreeNode parent,
        IQsiExpressionNode[] elements,
        IUserDataHolder userData)
    {
        Parent = parent;
        Elements = elements;
        UserData = userData;
    }
}