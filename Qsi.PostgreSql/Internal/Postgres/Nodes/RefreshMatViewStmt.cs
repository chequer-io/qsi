// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RefreshMatViewStmt")]
    internal class RefreshMatViewStmt : IPgTree
    {
        public bool concurrent { get; set; }

        public bool skipData { get; set; }

        public RangeVar relation { get; set; }
    }
}
