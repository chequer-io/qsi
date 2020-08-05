// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("TypeName")]
    internal class TypeName : IPgTree
    {
        public IPgTree[] names { get; set; }

        public int /* oid */ typeOid { get; set; }

        public bool setof { get; set; }

        public bool pct_type { get; set; }

        public IPgTree[] typmods { get; set; }

        public int typemod { get; set; }

        public IPgTree[] arrayBounds { get; set; }

        public int location { get; set; }
    }
}
