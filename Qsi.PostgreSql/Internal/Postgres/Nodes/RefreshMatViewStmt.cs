// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RefreshMatViewStmt")]
    internal class RefreshMatViewStmt : Node
    {
        public bool concurrent { get; set; }

        public bool skipData { get; set; }

        public RangeVar relation { get; set; }
    }
}
