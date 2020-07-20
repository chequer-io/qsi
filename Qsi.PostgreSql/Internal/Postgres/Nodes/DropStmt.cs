// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropStmt")]
    internal class DropStmt : Node
    {
        public IPgTree[] objects { get; set; }

        public ObjectType removeType { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }

        public bool concurrent { get; set; }
    }
}
