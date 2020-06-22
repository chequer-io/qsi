using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a joined table.
    /// </summary>
    public interface IQsiJoinedTable : IQsiTable
    {
        /// <summary>
        /// Get the left table.
        /// </summary>
        IQsiTable Left { get; }

        /// <summary>
        /// Get the join type.
        /// </summary>
        QsiJoinType JoinType { get; }

        /// <summary>
        /// Get the right table.
        /// </summary>
        IQsiTable Right { get; }

        /// <summary>
        /// Get the pivot columns declaration.
        /// </summary>
        IQsiColumnsDeclaration PivotColumns { get; }
    }
}
