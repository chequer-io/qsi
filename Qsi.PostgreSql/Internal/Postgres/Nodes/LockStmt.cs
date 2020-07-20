// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("LockStmt")]
    internal class LockStmt : Node
    {
        public IPgTree[] relations { get; set; }

        public int mode { get; set; }

        public bool nowait { get; set; }
    }
}
