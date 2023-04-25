using Qsi.Data;

namespace Qsi.Analyzers.Table
{
    public class QsiTableResult : IQsiAnalysisResult
    {
        public QsiTableStructure Table { get; }

        public QsiTableResult(QsiTableStructure table)
        {
            Table = table;
        }
    }
}
