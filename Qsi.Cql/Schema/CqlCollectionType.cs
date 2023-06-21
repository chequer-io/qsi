namespace Qsi.Cql.Schema;

public abstract class CqlCollectionType : CqlType
{
}

public sealed class CqlListType : CqlCollectionType
{
    public CqlType ElementType { get; }

    internal CqlListType(CqlType elementType)
    {
        ElementType = elementType;
    }

    public override string ToSql()
    {
        return $"list<{ElementType.ToSql()}>";
    }
}

public sealed class CqlSetType : CqlCollectionType
{
    public CqlType ElementType { get; }

    internal CqlSetType(CqlType elementType)
    {
        ElementType = elementType;
    }

    public override string ToSql()
    {
        return $"set<{ElementType.ToSql()}>";
    }
}

public sealed class CqlMapType : CqlCollectionType
{
    public CqlType KeyType { get; }

    public CqlType ValueType { get; }

    internal CqlMapType(CqlType keyType, CqlType valueType)
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    public override string ToSql()
    {
        return $"map<{KeyType.ToSql()}, {ValueType.ToSql()}>";
    }
}