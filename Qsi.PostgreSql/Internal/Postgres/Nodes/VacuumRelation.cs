// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("VacuumRelation")]
    internal class VacuumRelation : IPgTree
    {
        public RangeVar relation { get; set; }

        public int /* oid */ oid { get; set; }

        public IPgTree[] va_cols { get; set; }
    }
}
