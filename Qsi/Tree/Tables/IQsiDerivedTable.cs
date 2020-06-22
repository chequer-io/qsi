namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a derived table.
    /// </summary>
    public interface IQsiDerivedTable : IQsiTable, IQsiAliased
    {
        /// <summary>
        /// Get the table directives.
        /// </summary>
        IQsiTableDirectives Directives { get; }

        /// <summary>
        /// Get the columns declaration.
        /// </summary>
        IQsiColumnsDeclaration Columns { get; }

        /// <summary>
        /// Get the table source.
        /// </summary>
        IQsiTable Source { get; }
    }
}
