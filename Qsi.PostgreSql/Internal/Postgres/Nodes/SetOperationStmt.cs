// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("SetOperationStmt")]
    internal class SetOperationStmt : IPgTree
    {
        public SetOperation op { get; set; }

        public bool all { get; set; }

        public IPgTree larg { get; set; }

        public IPgTree rarg { get; set; }

        public IPgTree[] colTypes { get; set; }

        public IPgTree[] colTypmods { get; set; }

        public IPgTree[] colCollations { get; set; }

        public IPgTree[] groupClauses { get; set; }
    }
}
