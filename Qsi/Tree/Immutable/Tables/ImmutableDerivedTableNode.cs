using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDerivedTableNode : IQsiDerivedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableDirectivesNode Directives { get; }

        public IQsiColumnsDeclarationNode Columns { get; }

        public IQsiTableNode Source { get; }

        public IQsiAliasNode Alias { get; }

        public IQsiWhereExpressionNode WhereExpression { get; }

        public IQsiMultipleOrderExpressionNode OrderExpression { get; }

        public IQsiLimitExpressionNode LimitExpression { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => 
            TreeHelper.YieldChildren(Directives, Columns, Source, Alias, WhereExpression, OrderExpression, LimitExpression);

        public ImmutableDerivedTableNode(
            IQsiTreeNode parent,
            IQsiTableDirectivesNode directives,
            IQsiColumnsDeclarationNode columns,
            IQsiTableNode source,
            IQsiAliasNode alias,
            IQsiWhereExpressionNode whereExpression,
            IQsiMultipleOrderExpressionNode orderExpression,
            IQsiLimitExpressionNode limitExpression,
            IUserDataHolder userData)
        {
            Parent = parent;
            Directives = directives;
            Columns = columns;
            Source = source;
            Alias = alias;
            WhereExpression = whereExpression;
            OrderExpression = orderExpression;
            LimitExpression = limitExpression;
            UserData = userData;
        }
    }
}
