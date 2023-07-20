using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableTableFunctionNode : IQsiTableFunctionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiFunctionExpressionNode Member { get; }

    public IQsiParametersExpressionNode Parameters { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Member, Parameters);

    public ImmutableTableFunctionNode(
        IQsiTreeNode parent,
        IQsiFunctionExpressionNode member,
        IQsiParametersExpressionNode parameters,
        IUserDataHolder userData)
    {
        Parent = parent;
        Member = member;
        Parameters = parameters;
        UserData = userData;
    }
}