// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("VariableShowStmt")]
    internal class VariableShowStmt : IPgTree
    {
        public string name { get; set; }
    }
}
