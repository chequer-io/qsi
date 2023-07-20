namespace Qsi.Cql.Schema;

public sealed class CqlFrozenType : CqlType
{
    public CqlType ElementType { get; }

    internal CqlFrozenType(CqlType elementType)
    {
        ElementType = elementType;
    }

    public override string ToSql()
    {
        return $"frozen<{ElementType.ToSql()}>";
    }
}