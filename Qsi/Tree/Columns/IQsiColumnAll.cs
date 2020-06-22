using Qsi.Data;

namespace Qsi.Tree.Columns
{
    /// <summary>
    /// Specifies all columns.
    /// </summary>
    public interface IQsiColumnAll : IQsiColumn
    {
        /// <summary>
        /// Get the path of the target table to search the entire columns.
        /// </summary>
        public QsiQualifiedIdentifier Path { get; }
    }
}
