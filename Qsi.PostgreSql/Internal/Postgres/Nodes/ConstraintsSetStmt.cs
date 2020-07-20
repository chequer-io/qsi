// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ConstraintsSetStmt")]
    internal class ConstraintsSetStmt : Node
    {
        public IPgTree[] constraints { get; set; }

        public bool deferred { get; set; }
    }
}
