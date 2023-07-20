using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableDerivedTableNode : IQsiDerivedTableNode
{
    public IQsiTreeNode Parent { get; }

    public IQsiTableDirectivesNode Directives { get; }

    public IQsiColumnsDeclarationNode Columns { get; }

    public IQsiTableNode Source { get; }

    public IQsiAliasNode Alias { get; }

    public IQsiWhereExpressionNode Where { get; }

    public IQsiGroupingExpressionNode Grouping { get; }

    public IQsiMultipleOrderExpressionNode Order { get; }

    public IQsiLimitExpressionNode Limit { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children =>
        TreeHelper.YieldChildren(Directives, Columns, Source, Alias, Where, Grouping, Order, Limit);

    public ImmutableDerivedTableNode(
        IQsiTreeNode parent,
        IQsiTableDirectivesNode directives,
        IQsiColumnsDeclarationNode columns,
        IQsiTableNode source,
        IQsiAliasNode alias,
        IQsiWhereExpressionNode @where,
        IQsiGroupingExpressionNode grouping,
        IQsiMultipleOrderExpressionNode order,
        IQsiLimitExpressionNode limit,
        IUserDataHolder userData)
    {
        Parent = parent;
        Directives = directives;
        Columns = columns;
        Source = source;
        Alias = alias;
        Where = @where;
        Grouping = grouping;
        Order = order;
        Limit = limit;
        UserData = userData;
    }
}