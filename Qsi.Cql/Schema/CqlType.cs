namespace Qsi.Cql.Schema;

public abstract class CqlType
{
    internal CqlType()
    {
    }

    public abstract string ToSql();

    public override string ToString()
    {
        return ToSql();
    }
}