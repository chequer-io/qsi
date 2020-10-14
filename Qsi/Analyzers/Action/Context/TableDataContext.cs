using Qsi.Analyzers.Context;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Context
{
    public abstract class TableDataContext : AnalyzerContextWrapper
    {
        public QsiTableStructure Table { get; }

        protected TableDataContext(IAnalyzerContext context, QsiTableStructure table) : base(context)
        {
            Table = table;
        }
    }
}
