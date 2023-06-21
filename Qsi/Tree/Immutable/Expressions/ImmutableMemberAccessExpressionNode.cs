using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableMemberAccessExpressionNode : IQsiMemberAccessExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiExpressionNode Target { get; }

    public IQsiExpressionNode Member { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target, Member);

    public ImmutableMemberAccessExpressionNode(
        IQsiTreeNode parent, 
        IQsiExpressionNode target,
        IQsiExpressionNode member,
        IUserDataHolder userData)
    {
        Parent = parent;
        Target = target;
        Member = member;
        UserData = userData;
    }
}