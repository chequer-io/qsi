// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateSubscriptionStmt")]
    internal class CreateSubscriptionStmt : Node
    {
        public char subname { get; set; }

        public char conninfo { get; set; }

        public IPgTree[] publication { get; set; }

        public IPgTree[] options { get; set; }
    }
}
