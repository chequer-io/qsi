// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("FetchStmt")]
    internal class FetchStmt : IPgTree
    {
        public FetchDirection direction { get; set; }

        public long howMany { get; set; }

        public string portalname { get; set; }

        public bool ismove { get; set; }
    }
}
