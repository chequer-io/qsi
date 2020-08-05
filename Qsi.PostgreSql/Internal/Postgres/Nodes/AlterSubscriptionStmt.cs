// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterSubscriptionStmt")]
    internal class AlterSubscriptionStmt : IPgTree
    {
        public AlterSubscriptionType kind { get; set; }

        public string subname { get; set; }

        public string conninfo { get; set; }

        public IPgTree[] publication { get; set; }

        public IPgTree[] options { get; set; }
    }
}
