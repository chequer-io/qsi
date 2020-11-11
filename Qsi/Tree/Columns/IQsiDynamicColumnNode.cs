using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the dynamic column in IQsiTable.
    /// </summary>
    public interface IQsiDynamicColumnNode : IQsiColumnNode
    {
        /// <summary>
        /// Get the dynamic column name.
        /// </summary>
        QsiQualifiedIdentifier Name { get; }
    }
}
