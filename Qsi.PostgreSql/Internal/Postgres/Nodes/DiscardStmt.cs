// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DiscardStmt")]
    internal class DiscardStmt : Node
    {
        public DiscardMode target { get; set; }
    }
}
