using Qsi.Data;

namespace Qsi.Analyzers.Table
{
    public class QsiTableResult : IQsiAnalysisResult
    {
        public QsiTableStructure Table { get; }

        public virtual QsiSensitiveDataCollection SensitiveDataCollection => QsiSensitiveDataCollection.Empty;

        public QsiTableResult(QsiTableStructure table)
        {
            Table = table;
        }
    }
}
