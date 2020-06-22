namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a virtual table in which two or more tables are combined.
    /// </summary>
    public interface IQsiCompositeTable : IQsiTable
    {
        /// <summary>
        /// Get all tables.
        /// </summary>
        IQsiTable[] Tables { get; }
    }
}
