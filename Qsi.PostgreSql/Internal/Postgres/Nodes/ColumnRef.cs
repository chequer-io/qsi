// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ColumnRef")]
    internal class ColumnRef : Node
    {
        public IPgTree[] fields { get; set; }

        public int location { get; set; }
    }
}
