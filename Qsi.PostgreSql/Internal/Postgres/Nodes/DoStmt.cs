// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DoStmt")]
    internal class DoStmt : Node
    {
        public IPgTree[] args { get; set; }
    }
}
