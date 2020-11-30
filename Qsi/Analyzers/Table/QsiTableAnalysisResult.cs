using Qsi.Data;

namespace Qsi.Analyzers.Table
{
    public class QsiTableAnalysisResult : IQsiAnalysisResult
    {
        public QsiTableStructure Table { get; }

        public QsiTableAnalysisResult(QsiTableStructure table)
        {
            Table = table;
        }
    }
}
