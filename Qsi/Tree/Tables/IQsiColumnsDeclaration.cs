namespace Qsi.Tree
{
    /// <summary>
    /// The interface where the declared columns.
    /// </summary>
    public interface IQsiColumnsDeclaration : IQsiTreeNode
    {
        /// <summary>
        /// Get count of declared columns.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get all declared columns. 
        /// </summary>
        IQsiColumn[] Columns { get; }

        bool IsEmpty => Count <= 0;
    }
}
