using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableSetColumnExpressionNode : IQsiSetColumnExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public QsiQualifiedIdentifier Target { get; }

    public IQsiExpressionNode Value { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

    public ImmutableSetColumnExpressionNode(
        IQsiTreeNode parent,
        QsiQualifiedIdentifier target,
        IQsiExpressionNode value,
        IUserDataHolder userData)
    {
        Parent = parent;
        Target = target;
        Value = value;
        UserData = userData;
    }
}