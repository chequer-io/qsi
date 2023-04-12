namespace Qsi.PostgreSql.Data;

public class PgBinaryString
{
    public string Value { get; }

    public PgBinaryString(string value)
    {
        Value = value;
    }
}
