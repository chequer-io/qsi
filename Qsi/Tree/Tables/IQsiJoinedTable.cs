using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a joined table.
    /// </summary>
    public interface IQsiJoinedTable : IQsiTable
    {
        /// <summary>
        /// Get the left source.
        /// </summary>
        IQsiTable LeftSource { get; }

        /// <summary>
        /// Get the join type.
        /// </summary>
        QsiJoinType JoinType { get; }

        /// <summary>
        /// Get the right source.
        /// </summary>
        IQsiTable RightSource { get; }

        /// <summary>
        /// Get the pivot columns declaration.
        /// </summary>
        IQsiColumnsDeclaration PivotColumns { get; }
    }
}
