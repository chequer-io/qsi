using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableDerivedColumnNode : IQsiDerivedColumnNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiColumnNode Column { get; }

    public IQsiExpressionNode Expression { get; }

    public IQsiAliasNode Alias { get; }

    public QsiIdentifier InferredName { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Column, Expression, Alias);

    public ImmutableDerivedColumnNode(
        IQsiTreeNode parent,
        IQsiColumnNode column,
        IQsiExpressionNode expression,
        IQsiAliasNode alias,
        QsiIdentifier inferredName,
        IUserDataHolder userData)
    {
        Parent = parent;
        Column = column;
        Expression = expression;
        Alias = alias;
        InferredName = inferredName;
        UserData = userData;
    }
}