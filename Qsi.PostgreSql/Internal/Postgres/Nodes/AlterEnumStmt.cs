// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterEnumStmt")]
    internal class AlterEnumStmt : Node
    {
        public IPgTree[] typeName { get; set; }

        public string oldVal { get; set; }

        public string newVal { get; set; }

        public string newValNeighbor { get; set; }

        public bool newValIsAfter { get; set; }

        public bool skipIfNewValExists { get; set; }
    }
}
