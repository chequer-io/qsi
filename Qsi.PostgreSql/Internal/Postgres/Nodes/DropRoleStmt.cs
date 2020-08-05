// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropRoleStmt")]
    internal class DropRoleStmt : IPgTree
    {
        public IPgTree[] roles { get; set; }

        public bool missing_ok { get; set; }
    }
}
