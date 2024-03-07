namespace Qsi.Tree;

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
    string JoinType { get; }

    /// <summary>
    /// Gets whether natural join
    /// </summary>
    bool IsNatural { get; }

    /// <summary>
    /// Gets whether comma join
    /// </summary>
    bool IsComma { get; }

    /// <summary>
    /// Get the right table.
    /// </summary>
    IQsiTableNode Right { get; }

    /// <summary>
    /// Get the pivot columns declaration.
    /// </summary>
    IQsiColumnsDeclarationNode PivotColumns { get; }

    /// <summary>
    /// Get the pivot expression declaration.
    /// </summary>
    IQsiExpressionNode PivotExpression { get; }
}