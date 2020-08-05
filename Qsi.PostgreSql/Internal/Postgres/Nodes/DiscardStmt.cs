// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DiscardStmt")]
    internal class DiscardStmt : IPgTree
    {
        public DiscardMode target { get; set; }
    }
}
