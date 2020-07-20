// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterRoleStmt")]
    internal class AlterRoleStmt : Node
    {
        public RoleSpec role { get; set; }

        public IPgTree[] options { get; set; }

        public int action { get; set; }
    }
}
