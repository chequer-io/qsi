// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterObjectDependsStmt")]
    internal class AlterObjectDependsStmt : IPgTree
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public IPgTree @object { get; set; }

        public PgValue extname { get; set; }

        public bool remove { get; set; }
    }
}
