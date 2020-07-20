// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterDefaultPrivilegesStmt")]
    internal class AlterDefaultPrivilegesStmt : Node
    {
        public IPgTree[] options { get; set; }

        public GrantStmt action { get; set; }
    }
}
