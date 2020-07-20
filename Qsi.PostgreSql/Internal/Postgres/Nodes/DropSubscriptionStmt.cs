// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropSubscriptionStmt")]
    internal class DropSubscriptionStmt : Node
    {
        public string subname { get; set; }

        public bool missing_ok { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
