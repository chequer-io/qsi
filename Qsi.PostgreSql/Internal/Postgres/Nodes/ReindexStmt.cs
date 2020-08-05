// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ReindexStmt")]
    internal class ReindexStmt : IPgTree
    {
        public ReindexObjectType kind { get; set; }

        public RangeVar relation { get; set; }

        public string name { get; set; }

        public int options { get; set; }

        public bool concurrent { get; set; }
    }
}
