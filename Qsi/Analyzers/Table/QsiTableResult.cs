using Qsi.Data;

namespace Qsi.Analyzers.Table
{
    public class QsiTableResult : IQsiAnalysisResult
    {
        public QsiTableStructure Table { get; }

        public virtual QsiSensitiveDataHolder SensitiveDataHolder => null;

        public QsiTableResult(QsiTableStructure table)
        {
            Table = table;
        }
    }
}
