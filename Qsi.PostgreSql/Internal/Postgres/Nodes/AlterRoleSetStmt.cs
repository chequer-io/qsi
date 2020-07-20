// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterRoleSetStmt")]
    internal class AlterRoleSetStmt : Node
    {
        public RoleSpec role { get; set; }

        public char database { get; set; }

        public VariableSetStmt setstmt { get; set; }
    }
}
