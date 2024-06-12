using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Data;

namespace Qsi.Engines.Explain;

public sealed class QsiExplainDataManipulationResult : IQsiAnalysisResult
{
    public QsiTableStructure Table { get; }

    public QsiTableColumn[] AffectedColumns { get; }

    public QsiDataValueOperation[] Operations { get; }

    public QsiSensitiveDataCollection SensitiveDataCollection { get; } = new();

    public ICollection<QsiTableStructure> TablesInRows { get; }

    public QsiExplainDataManipulationResult(
        QsiTableStructure table,
        QsiTableColumn[] affectedColumns,
        QsiDataValueOperation[] operations)
        : this(table, affectedColumns, operations, Array.Empty<QsiTableStructure>())
    {
    }

    public QsiExplainDataManipulationResult(
        QsiTableStructure table,
        QsiTableColumn[] affectedColumns,
        QsiDataValueOperation[] operations,
        ICollection<QsiTableStructure> tablesInRows)
    {
        Table = table;
        AffectedColumns = affectedColumns;
        Operations = operations;
        TablesInRows = tablesInRows;
    }
}
