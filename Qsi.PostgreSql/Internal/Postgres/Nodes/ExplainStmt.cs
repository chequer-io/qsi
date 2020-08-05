// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ExplainStmt")]
    internal class ExplainStmt : IPgTree
    {
        public IPgTree query { get; set; }

        public IPgTree[] options { get; set; }
    }
}
