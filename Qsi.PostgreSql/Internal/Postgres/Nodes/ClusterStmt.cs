// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ClusterStmt")]
    internal class ClusterStmt : Node
    {
        public RangeVar relation { get; set; }

        public string indexname { get; set; }

        public int options { get; set; }
    }
}
