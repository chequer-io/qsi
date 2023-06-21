using System.Collections.Generic;

namespace Qsi.Tree;

/// <summary>
/// The interface where the declared columns.
/// </summary>
public interface IQsiColumnsDeclarationNode : IQsiTreeNode, IEnumerable<IQsiColumnNode>
{
    /// <summary>
    /// Get count of declared columns.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Get all declared columns. 
    /// </summary>
    IQsiColumnNode[] Columns { get; }

    bool IsEmpty { get; }
}