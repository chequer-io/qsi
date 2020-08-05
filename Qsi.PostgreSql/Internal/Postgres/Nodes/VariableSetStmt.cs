// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("VariableSetStmt")]
    internal class VariableSetStmt : IPgTree
    {
        public VariableSetKind kind { get; set; }

        public string name { get; set; }

        public IPgTree[] args { get; set; }

        public bool is_local { get; set; }
    }
}
