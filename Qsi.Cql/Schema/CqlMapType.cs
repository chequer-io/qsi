namespace Qsi.Cql.Schema
{
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
}
