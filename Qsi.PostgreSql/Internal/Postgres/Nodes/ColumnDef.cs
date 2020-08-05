// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ColumnDef")]
    internal class ColumnDef : IPgTree
    {
        public string colname { get; set; }

        public TypeName typeName { get; set; }

        public int inhcount { get; set; }

        public bool is_local { get; set; }

        public bool is_not_null { get; set; }

        public bool is_from_type { get; set; }

        public char storage { get; set; }

        public IPgTree raw_default { get; set; }

        public IPgTree cooked_default { get; set; }

        public char identity { get; set; }

        public RangeVar identitySequence { get; set; }

        public char generated { get; set; }

        public CollateClause collClause { get; set; }

        public int /* oid */ collOid { get; set; }

        public IPgTree[] constraints { get; set; }

        public IPgTree[] fdwoptions { get; set; }

        public int location { get; set; }
    }
}
