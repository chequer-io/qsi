using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies all columns.
    /// </summary>
    public interface IQsiAllColumnNode : IQsiColumnNode
    {
        /// <summary>
        /// Get the path of the target table to search the entire columns.
        /// </summary>
        public QsiQualifiedIdentifier Path { get; }

        public bool IncludeInvisibleColumns { get; }
    }
}
