// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterObjectSchemaStmt")]
    internal class AlterObjectSchemaStmt : IPgTree
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public IPgTree @object { get; set; }

        public string newschema { get; set; }

        public bool missing_ok { get; set; }
    }
}
