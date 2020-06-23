using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column declared in IQsiTable.
    /// </summary>
    public interface IQsiDeclaredColumnNode : IQsiColumnNode
    {
        /// <summary>
        /// Get the declared column name.
        /// </summary>
        QsiIdentifier Name { get; }
    }
}
