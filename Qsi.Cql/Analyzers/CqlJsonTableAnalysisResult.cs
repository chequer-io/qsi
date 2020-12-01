using Qsi.Analyzers.Table;
using Qsi.Data;

namespace Qsi.Cql.Analyzers
{
    public sealed class CqlJsonTableAnalysisResult : QsiTableAnalysisResult
    {
        public CqlJsonTableAnalysisResult(QsiTableStructure table) : base(table)
        {
        }
    }
}
