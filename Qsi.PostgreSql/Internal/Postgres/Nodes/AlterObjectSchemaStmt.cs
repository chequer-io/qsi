// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterObjectSchemaStmt")]
    internal class AlterObjectSchemaStmt : Node
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public Node @object { get; set; }

        public string newschema { get; set; }

        public bool missing_ok { get; set; }
    }
}
