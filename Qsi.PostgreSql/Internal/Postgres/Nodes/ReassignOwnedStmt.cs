// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ReassignOwnedStmt")]
    internal class ReassignOwnedStmt : Node
    {
        public IPgTree[] roles { get; set; }

        public RoleSpec newrole { get; set; }
    }
}
