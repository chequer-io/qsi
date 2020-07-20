// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RawStmt")]
    internal class RawStmt : Node
    {
        public Node stmt { get; set; }

        public int stmt_location { get; set; }

        public int stmt_len { get; set; }
    }
}
