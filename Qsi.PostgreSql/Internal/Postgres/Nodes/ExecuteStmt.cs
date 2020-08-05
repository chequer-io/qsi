// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ExecuteStmt")]
    internal class ExecuteStmt : IPgTree
    {
        public string name { get; set; }

        public IPgTree[] @params { get; set; }
    }
}
