using Qsi.Data;

namespace Qsi.Tree
{
    /// <summary>
    /// Specifies a joined table.
    /// </summary>
    public interface IQsiJoinedTableNode : IQsiTableNode
    {
        /// <summary>
        /// Get the left table.
        /// </summary>
        IQsiTableNode Left { get; }

        /// <summary>
        /// Get the join type.
        /// </summary>
        QsiJoinType JoinType { get; }

        /// <summary>
        /// Get the right table.
        /// </summary>
        IQsiTableNode Right { get; }

        /// <summary>
        /// Get the pivot columns declaration.
        /// </summary>
        IQsiColumnsDeclarationNode PivotColumns { get; }
    }
}
