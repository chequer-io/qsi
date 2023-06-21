using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableLiteralExpressionNode : IQsiLiteralExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public object Value { get; }

    public QsiDataType Type { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

    public ImmutableLiteralExpressionNode(IQsiTreeNode parent, object value, QsiDataType type, IUserDataHolder userData)
    {
        Parent = parent;
        Value = value;
        Type = type;
        UserData = userData;
    }
}