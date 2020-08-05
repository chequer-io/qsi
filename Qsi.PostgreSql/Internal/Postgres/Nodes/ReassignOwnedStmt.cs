// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ReassignOwnedStmt")]
    internal class ReassignOwnedStmt : IPgTree
    {
        public IPgTree[] roles { get; set; }

        public RoleSpec newrole { get; set; }
    }
}
