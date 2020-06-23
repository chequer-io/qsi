namespace Qsi.Tree
{
    /// <summary>
    /// The interface where the declared columns.
    /// </summary>
    public interface IQsiColumnsDeclarationNode : IQsiTreeNode
    {
        /// <summary>
        /// Get count of declared columns.
        /// </summary>
        int Count => Columns?.Length ?? 0;

        /// <summary>
        /// Get all declared columns. 
        /// </summary>
        IQsiColumnNode[] Columns { get; }

        bool IsEmpty => Count <= 0;
    }
}
