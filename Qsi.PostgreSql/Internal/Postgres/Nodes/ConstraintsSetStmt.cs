// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ConstraintsSetStmt")]
    internal class ConstraintsSetStmt : IPgTree
    {
        public IPgTree[] constraints { get; set; }

        public bool deferred { get; set; }
    }
}
