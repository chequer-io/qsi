// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeTableSample")]
    internal class RangeTableSample : IPgTree
    {
        public IPgTree relation { get; set; }

        public IPgTree[] method { get; set; }

        public IPgTree[] args { get; set; }

        public IPgTree repeatable { get; set; }

        public int location { get; set; }
    }
}
