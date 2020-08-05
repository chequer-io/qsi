// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropStmt")]
    internal class DropStmt : IPgTree
    {
        public IPgTree[] objects { get; set; }

        public ObjectType removeType { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }

        public bool concurrent { get; set; }
    }
}
