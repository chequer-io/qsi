using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the declared table.
    /// </summary>
    public interface IQsiTableAccess : IQsiTable
    {
        /// <summary>
        /// Get declared table name.
        /// </summary>
        QsiQualifiedIdentifier Name { get; }
    }
}
