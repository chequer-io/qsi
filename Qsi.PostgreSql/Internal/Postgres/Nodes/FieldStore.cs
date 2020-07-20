// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class FieldStore
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public IPgTree[] newvals { get; set; }

        public IPgTree[] fieldnums { get; set; }

        public string /* oid */ resulttype { get; set; }
    }
}
