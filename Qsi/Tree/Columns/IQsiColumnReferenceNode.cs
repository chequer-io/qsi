using Qsi.Data;

namespace Qsi.Tree;

/// <summary>
/// Specifies the column declared in IQsiTable.
/// </summary>
public interface IQsiColumnReferenceNode : IQsiColumnNode
{
    /// <summary>
    /// Get the declared column name.
    /// </summary>
    QsiQualifiedIdentifier Name { get; }
}