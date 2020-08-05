// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropSubscriptionStmt")]
    internal class DropSubscriptionStmt : IPgTree
    {
        public string subname { get; set; }

        public bool missing_ok { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
