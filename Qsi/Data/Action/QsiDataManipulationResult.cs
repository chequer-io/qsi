using System;
using System.Collections.Generic;
using Qsi.Analyzers;

namespace Qsi.Data;

public class QsiDataManipulationResult : IQsiAnalysisResult
{
    public QsiTableStructure Table { get; set; }

    public QsiTableColumn[] AffectedColumns { get; set; }

    public QsiDataRowCollection InsertRows { get; set; }

    public QsiDataRowCollection DuplicateRows { get; set; }

    public QsiDataRowCollection UpdateBeforeRows { get; set; }

    public QsiDataRowCollection UpdateAfterRows { get; set; }

    public QsiDataRowCollection DeleteRows { get; set; }

    public ICollection<QsiTableStructure> TablesInRows { get; set; } = Array.Empty<QsiTableStructure>();

    // Note: If we need to support the "DML Snapshot" feature for the LOAD DATA statement or any new statements that manipulate data using files,
    //       we will need to refactor this field or class to be more abstract.
    public QsiIdentifier FileValue { get; set; }

    public QsiSensitiveDataCollection SensitiveDataCollection { get; } = new();
}
