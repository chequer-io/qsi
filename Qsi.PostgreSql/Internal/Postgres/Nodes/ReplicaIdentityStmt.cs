// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ReplicaIdentityStmt")]
    internal class ReplicaIdentityStmt : Node
    {
        public char identity_type { get; set; }

        public char name { get; set; }
    }
}
