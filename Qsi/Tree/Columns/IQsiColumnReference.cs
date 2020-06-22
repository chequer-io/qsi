using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column defined in IQsiTableReference.
    /// </summary>
    public interface IQsiColumnReference : IQsiColumn
    {
        /// <summary>
        /// Get the column name.
        /// </summary>
        QsiIdentifier Name { get; }
    }
}
