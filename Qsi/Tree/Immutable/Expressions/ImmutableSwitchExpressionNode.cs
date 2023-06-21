using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableSwitchExpressionNode : IQsiSwitchExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiExpressionNode Value { get; }

    public IQsiSwitchCaseExpressionNode[] Cases { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (Value != null)
                yield return Value;

            foreach (var @case in Cases)
                yield return @case;
        }
    }

    public ImmutableSwitchExpressionNode(IQsiTreeNode parent, IQsiExpressionNode value, IQsiSwitchCaseExpressionNode[] cases, IUserDataHolder userData)
    {
        Parent = parent;
        Value = value;
        Cases = cases;
        UserData = userData;
    }
}