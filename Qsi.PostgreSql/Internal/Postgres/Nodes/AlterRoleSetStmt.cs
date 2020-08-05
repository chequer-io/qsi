// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterRoleSetStmt")]
    internal class AlterRoleSetStmt : IPgTree
    {
        public RoleSpec role { get; set; }

        public string database { get; set; }

        public VariableSetStmt setstmt { get; set; }
    }
}
