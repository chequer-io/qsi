using Qsi.Analyzers.Action.Cache;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Context;

public sealed class TableDataInsertContext : TableDataContext
{
    public DataManipulationTarget[] Targets { get; set; }

    public ColumnTarget[] ColumnTargets { get; set; }

    public DmlTableStructureCache TableStructureCache { get; } = new();

    public TableDataInsertContext(IAnalyzerContext context, QsiTableStructure table) : base(context, table)
    {
    }
}
