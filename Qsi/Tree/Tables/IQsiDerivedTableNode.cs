namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a derived table.
    /// </summary>
    public interface IQsiDerivedTableNode : IQsiTableNode, IQsiAliasedNode
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
    }
}
