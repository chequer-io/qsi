using Qsi.Data;

namespace Qsi.PostgreSql.Data;

internal readonly struct PostgreSqlInteger : IDataProvider<int>
{
    public QsiDataType Type => QsiDataType.Numeric;

    public PostgreSqlIntegerKind Kind { get; }
    
    public int Value { get; }

    public PostgreSqlInteger(PostgreSqlIntegerKind kind, string value)
    {
        throw new System.NotImplementedException();
    }
}
