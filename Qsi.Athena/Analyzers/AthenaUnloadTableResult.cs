using Qsi.Analyzers.Table;
using Qsi.Data;

namespace Qsi.Athena.Analyzers;

public sealed class AthenaUnloadTableResult : QsiTableResult
{
    public AthenaUnloadTableResult(QsiTableStructure table) : base(table)
    {
    }
}
