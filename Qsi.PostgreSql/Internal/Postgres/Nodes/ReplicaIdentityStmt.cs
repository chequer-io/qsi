// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ReplicaIdentityStmt")]
    internal class ReplicaIdentityStmt : IPgTree
    {
        public char identity_type { get; set; }

        public string name { get; set; }
    }
}
