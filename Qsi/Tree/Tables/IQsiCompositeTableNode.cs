namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a virtual table in which two or more tables are combined.
    /// </summary>
    public interface IQsiCompositeTableNode : IQsiTableNode
    {
        /// <summary>
        /// Get all tables.
        /// </summary>
        IQsiTableNode[] Sources { get; }

        IQsiMultipleOrderExpressionNode OrderExpression { get; }

        IQsiLimitExpressionNode LimitExpression { get; }
    }
}
