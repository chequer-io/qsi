// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DeclareCursorStmt")]
    internal class DeclareCursorStmt : IPgTree
    {
        public string portalname { get; set; }

        public int options { get; set; }

        public IPgTree query { get; set; }
    }
}
