using Qsi.Data;

namespace Qsi.Tree;

/// <summary>
/// Specifies table definition.
/// </summary>
public interface IQsiTableDefinitionNode : IQsiDefinitionNode
{
    /// <summary>
    /// Get declared table name.
    /// </summary>
    QsiQualifiedIdentifier Identifier { get; }

    /// <summary>
    /// Get the definition conflict behavior
    /// </summary>
    QsiDefinitionConflictBehavior ConflictBehavior { get; }

    /// <summary>
    /// Get the columns declaration.
    /// </summary>
    IQsiColumnsDeclarationNode Columns { get; }

    /// <summary>
    /// Get the column source.
    /// </summary>
    IQsiTableNode ColumnSource { get; }

    /// <summary>
    /// Get the data source.
    /// </summary>
    IQsiTableNode DataSource { get; }
}