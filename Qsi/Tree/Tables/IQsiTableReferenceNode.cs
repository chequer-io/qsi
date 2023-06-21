using Qsi.Data;

namespace Qsi.Tree;

/// <summary>
/// Specifies the declared table.
/// </summary>
public interface IQsiTableReferenceNode : IQsiTableNode
{
    /// <summary>
    /// Get declared table name.
    /// </summary>
    QsiQualifiedIdentifier Identifier { get; }
}