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
    
    public QsiIdentifier FileValue { get; set; }

    public QsiSensitiveDataCollection SensitiveDataCollection { get; } = new();
}
