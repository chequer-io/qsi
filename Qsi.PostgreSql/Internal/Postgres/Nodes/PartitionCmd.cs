// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PartitionCmd")]
    internal class PartitionCmd : IPgTree
    {
        public RangeVar name { get; set; }

        public PartitionBoundSpec bound { get; set; }
    }
}
