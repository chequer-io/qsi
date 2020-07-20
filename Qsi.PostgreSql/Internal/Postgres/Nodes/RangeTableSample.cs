// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTableSample")]
    internal class RangeTableSample : Node
    {
        public Node relation { get; set; }

        public IPgTree[] method { get; set; }

        public IPgTree[] args { get; set; }

        public Node repeatable { get; set; }

        public int location { get; set; }
    }
}
