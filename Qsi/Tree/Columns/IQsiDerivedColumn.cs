namespace Qsi.Tree
{
    /// <summary>
    /// Specifies an expression or aliased column.
    /// </summary>
    public interface IQsiDerivedColumn : IQsiAliasedColumn
    {
        /// <summary>
        /// Get the target column.
        /// </summary>
        IQsiColumn Column { get; }

        /// <summary>
        /// Get the expression in column.
        /// </summary>
        IQsiExpression Expression { get; }

        bool IsColumn => Column != null;

        bool IsExpression => Expression != null;
    }
}
