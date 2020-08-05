// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RenameStmt")]
    internal class RenameStmt : IPgTree
    {
        public ObjectType renameType { get; set; }

        public ObjectType relationType { get; set; }

        public RangeVar relation { get; set; }

        public IPgTree @object { get; set; }

        public string subname { get; set; }

        public string newname { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
