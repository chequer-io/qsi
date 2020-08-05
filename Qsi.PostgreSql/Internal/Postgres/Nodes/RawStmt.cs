// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RawStmt")]
    internal class RawStmt : IPgTree
    {
        public IPgTree stmt { get; set; }

        public int stmt_location { get; set; }

        public int stmt_len { get; set; }
    }
}
