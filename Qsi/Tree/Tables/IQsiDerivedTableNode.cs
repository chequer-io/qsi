namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a derived table.
    /// </summary>
    public interface IQsiDerivedTableNode : IQsiTableNode
    {
        /// <summary>
        /// Get the table directives.
        /// </summary>
        IQsiTableDirectivesNode Directives { get; }

        /// <summary>
        /// Get the columns declaration.
        /// </summary>
        IQsiColumnsDeclarationNode Columns { get; }

        /// <summary>
        /// Get the table source.
        /// </summary>
        IQsiTableNode Source { get; }

        /// <summary>
        /// Get the delcared alias.
        /// </summary>
        IQsiAliasNode Alias { get; }

        /// <summary>
        /// Get the where clause.
        /// </summary>
        IQsiWhereExpressionNode WhereExpression { get; }

        /// <summary>
        /// Get the grouping clause.
        /// </summary>
        IQsiGroupingExpressionNode GroupingExpression { get; }

        /// <summary>
        /// Get the order clause.
        /// </summary>
        IQsiMultipleOrderExpressionNode OrderExpression { get; }

        /// <summary>
        /// Get the limit clause.
        /// </summary>
        IQsiLimitExpressionNode LimitExpression { get; }
    }
}
