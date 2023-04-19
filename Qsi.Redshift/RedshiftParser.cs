using Qsi.PostgreSql;

namespace Qsi.Redshift;

public sealed class RedshiftParser : PostgreSqlParser
{
    public RedshiftParser(int totalStack, ulong totalMemory) : base(totalStack, totalMemory)
    {
    }
}
