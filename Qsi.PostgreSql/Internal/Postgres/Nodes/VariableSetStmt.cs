// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("VariableSetStmt")]
    internal class VariableSetStmt : Node
    {
        public VariableSetKind kind { get; set; }

        public char name { get; set; }

        public IPgTree[] args { get; set; }

        public bool is_local { get; set; }
    }
}
