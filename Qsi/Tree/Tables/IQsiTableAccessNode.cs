using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the declared table.
    /// </summary>
    public interface IQsiTableAccessNode : IQsiTableNode
    {
        /// <summary>
        /// Get declared table name.
        /// </summary>
        QsiQualifiedIdentifier Name { get; }
    }
}
