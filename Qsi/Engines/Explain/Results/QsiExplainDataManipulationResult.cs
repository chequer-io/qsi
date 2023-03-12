using Qsi.Analyzers;
using Qsi.Data;

namespace Qsi.Engines.Explain
{
    public sealed class QsiExplainDataManipulationResult : IQsiAnalysisResult
    {
        public QsiTableStructure Table { get; }

        public QsiTableColumn[] AffectedColumns { get; }

        public QsiDataValueOperation[] Operations { get; }

        public QsiSensitiveDataHolder SensitiveDataHolder => null;

        public QsiExplainDataManipulationResult(QsiTableStructure table, QsiTableColumn[] affectedColumns, QsiDataValueOperation[] operations)
        {
            Table = table;
            AffectedColumns = affectedColumns;
            Operations = operations;
        }
    }
}
