namespace Qsi.Tree
{
    /// <summary>
    /// Specifies table directives.
    /// </summary>
    public interface IQsiTableDirectivesNode : IQsiTreeNode
    {
        /// <summary>
        /// Get delcared tables.
        /// </summary>
        IQsiDerivedTableNode[] Tables { get; }

        /// <summary>
        /// Gets whether recursion is supported.
        /// </summary>
        bool IsRecursive { get; }
    }
}
