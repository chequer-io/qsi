using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableRowValueExpressionNode : IQsiRowValueExpressionNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiExpressionNode[] ColumnValues { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => ColumnValues;

    public ImmutableRowValueExpressionNode(IQsiTreeNode parent, IQsiExpressionNode[] columnValues, IUserDataHolder userData)
    {
        Parent = parent;
        ColumnValues = columnValues;
        UserData = userData;
    }
}