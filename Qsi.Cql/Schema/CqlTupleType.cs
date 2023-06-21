namespace Qsi.Cql.Schema;

public sealed class CqlTupleType : CqlType
{
    public CqlType FirstType { get; }

    public CqlType SecondType { get; }

    public CqlTupleType(CqlType firstType, CqlType secondType)
    {
        FirstType = firstType;
        SecondType = secondType;
    }

    public override string ToSql()
    {
        return $"tuple<{FirstType.ToSql()}, {SecondType.ToSql()}>";
    }
}