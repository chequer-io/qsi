using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column declared in IQsiTable.
    /// </summary>
    public interface IQsiDeclaredColumn : IQsiColumn
    {
        /// <summary>
        /// Get the declared column name.
        /// </summary>
        QsiIdentifier Name { get; }
    }
}
