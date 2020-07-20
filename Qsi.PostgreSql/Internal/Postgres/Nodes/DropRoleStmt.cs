// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropRoleStmt")]
    internal class DropRoleStmt : Node
    {
        public IPgTree[] roles { get; set; }

        public bool missing_ok { get; set; }
    }
}
