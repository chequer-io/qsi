// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ReindexStmt")]
    internal class ReindexStmt : Node
    {
        public ReindexObjectType kind { get; set; }

        public RangeVar relation { get; set; }

        public string name { get; set; }

        public int options { get; set; }

        public bool concurrent { get; set; }
    }
}
