namespace Qsi.Cql.Schema
{
    public sealed class CqlListType : CqlType
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
}
