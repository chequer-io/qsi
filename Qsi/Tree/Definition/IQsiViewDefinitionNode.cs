﻿using Qsi.Data;

namespace Qsi.Tree;

/// <summary>
/// Specifies view definition.
/// </summary>
public interface IQsiViewDefinitionNode : IQsiDefinitionNode
{
    /// <summary>
    /// Get the table directives.
    /// </summary>
    IQsiTableDirectivesNode Directives { get; }

    /// <summary>
    /// Get the definition conflict behavior
    /// </summary>
    QsiDefinitionConflictBehavior ConflictBehavior { get; }

    /// <summary>
    /// Get declared view name.
    /// </summary>
    QsiQualifiedIdentifier Identifier { get; }

    /// <summary>
    /// Get the columns declaration.
    /// </summary>
    IQsiColumnsDeclarationNode Columns { get; }

    /// <summary>
    /// Get the table source.
    /// </summary>
    IQsiTableNode Source { get; }
}