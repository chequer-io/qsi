namespace Qsi.Tree
{
    /// <summary>
    /// Specifies table directives.
    /// </summary>
    public interface IQsiTableDirectives : IQsiTreeNode
    {
        /// <summary>
        /// Get delcared tables.
        /// </summary>
        IQsiTable[] Tables { get; }
    }
}
