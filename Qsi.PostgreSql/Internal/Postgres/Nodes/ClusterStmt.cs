// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ClusterStmt")]
    internal class ClusterStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public string indexname { get; set; }

        public int options { get; set; }
    }
}
