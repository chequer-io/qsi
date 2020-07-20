// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("PrepareStmt")]
    internal class PrepareStmt : Node
    {
        public string name { get; set; }

        public IPgTree[] argtypes { get; set; }

        public Node query { get; set; }
    }
}
