// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterDefaultPrivilegesStmt")]
    internal class AlterDefaultPrivilegesStmt : IPgTree
    {
        public IPgTree[] options { get; set; }

        public GrantStmt action { get; set; }
    }
}
