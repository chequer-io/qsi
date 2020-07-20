// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ExplainStmt")]
    internal class ExplainStmt : Node
    {
        public Node query { get; set; }

        public IPgTree[] options { get; set; }
    }
}
