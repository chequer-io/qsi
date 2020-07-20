// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("FetchStmt")]
    internal class FetchStmt : Node
    {
        public FetchDirection direction { get; set; }

        public long howMany { get; set; }

        public char portalname { get; set; }

        public bool ismove { get; set; }
    }
}
