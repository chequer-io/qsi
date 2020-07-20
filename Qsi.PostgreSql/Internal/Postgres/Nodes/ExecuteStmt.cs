// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ExecuteStmt")]
    internal class ExecuteStmt : Node
    {
        public string name { get; set; }

        public IPgTree[] @params { get; set; }
    }
}
