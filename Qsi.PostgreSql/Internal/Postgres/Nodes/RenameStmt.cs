// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RenameStmt")]
    internal class RenameStmt : Node
    {
        public ObjectType renameType { get; set; }

        public ObjectType relationType { get; set; }

        public RangeVar relation { get; set; }

        public Node @object { get; set; }

        public char subname { get; set; }

        public char newname { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
