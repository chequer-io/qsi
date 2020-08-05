// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropOwnedStmt")]
    internal class DropOwnedStmt : IPgTree
    {
        public IPgTree[] roles { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
