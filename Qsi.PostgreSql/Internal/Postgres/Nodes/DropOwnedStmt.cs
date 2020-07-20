// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropOwnedStmt")]
    internal class DropOwnedStmt : Node
    {
        public IPgTree[] roles { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
