// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DeclareCursorStmt")]
    internal class DeclareCursorStmt : Node
    {
        public char portalname { get; set; }

        public int options { get; set; }

        public Node query { get; set; }
    }
}
