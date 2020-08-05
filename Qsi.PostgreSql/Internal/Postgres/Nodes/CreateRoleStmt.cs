// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateRoleStmt")]
    internal class CreateRoleStmt : IPgTree
    {
        public RoleStmtType stmt_type { get; set; }

        public string role { get; set; }

        public IPgTree[] options { get; set; }
    }
}
