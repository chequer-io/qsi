// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DoStmt")]
    internal class DoStmt : IPgTree
    {
        public IPgTree[] args { get; set; }
    }
}
