using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Context
{
    public sealed class TableDataInsertContext : TableDataContext
    {
        public TableDataInsertTarget[] Targets { get; set; }

        public QsiIdentifier[] ColumnNames { get; set; }

        public int[] AffectedIndices { get; set; }

        public TableDataInsertContext(IAnalyzerContext context, QsiTableStructure table) : base(context, table)
        {
        }
    }
}
