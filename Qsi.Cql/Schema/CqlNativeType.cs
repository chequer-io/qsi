namespace Qsi.Cql.Schema
{
    public sealed class CqlNativeType : CqlType
    {
        public CqlDataType Type { get; }

        internal CqlNativeType(CqlDataType type)
        {
            Type = type;
        }

        public override string ToSql()
        {
            return Type.ToString().ToLower();
        }
    }
}
