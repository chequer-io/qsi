// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterObjectDependsStmt")]
    internal class AlterObjectDependsStmt : Node
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public Node @object { get; set; }

        public Value extname { get; set; }

        public bool remove { get; set; }
    }
}
