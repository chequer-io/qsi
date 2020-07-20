// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("GrantRoleStmt")]
    internal class GrantRoleStmt : Node
    {
        public IPgTree[] granted_roles { get; set; }

        public IPgTree[] grantee_roles { get; set; }

        public bool is_grant { get; set; }

        public bool admin_opt { get; set; }

        public RoleSpec grantor { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
