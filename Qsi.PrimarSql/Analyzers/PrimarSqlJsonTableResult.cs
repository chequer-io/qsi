using Qsi.Analyzers.Table;
using Qsi.Data;

namespace Qsi.PrimarSql.Analyzers;

public sealed class PrimarSqlJsonTableResult : QsiTableResult
{
    public PrimarSqlJsonTableResult(QsiTableStructure table) : base(table)
    {
    }
}