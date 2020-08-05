// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ColumnRef")]
    internal class ColumnRef : IPgTree
    {
        public IPgTree[] fields { get; set; }

        public int location { get; set; }
    }
}
