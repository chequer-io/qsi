using System.Collections.Generic;
using System.Linq;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableInlineDerivedTableNode : IQsiInlineDerivedTableNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiAliasNode Alias { get; }

    public IQsiColumnsDeclarationNode Columns { get; }

    public IQsiRowValueExpressionNode[] Rows { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Alias, Columns).Concat(Rows);

    public ImmutableInlineDerivedTableNode(
        IQsiTreeNode parent,
        IQsiAliasNode alias,
        IQsiColumnsDeclarationNode columns,
        IQsiRowValueExpressionNode[] rows, 
        IUserDataHolder userData)
    {
        Parent = parent;
        Alias = alias;
        Columns = columns;
        Rows = rows;
        UserData = userData;
    }
}