// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("SetOperationStmt")]
    internal class SetOperationStmt : Node
    {
        public SetOperation op { get; set; }

        public bool all { get; set; }

        public Node larg { get; set; }

        public Node rarg { get; set; }

        public IPgTree[] colTypes { get; set; }

        public IPgTree[] colTypmods { get; set; }

        public IPgTree[] colCollations { get; set; }

        public IPgTree[] groupClauses { get; set; }
    }
}
