using Qsi.Data;

namespace Qsi.Cql.Schema
{
    public sealed class CqlUserType : CqlType
    {
        public QsiQualifiedIdentifier Identifier { get; }

        internal CqlUserType(QsiQualifiedIdentifier identifier)
        {
            Identifier = identifier;
        }

        public override string ToSql()
        {
            return Identifier.ToString();
        }
    }
}
