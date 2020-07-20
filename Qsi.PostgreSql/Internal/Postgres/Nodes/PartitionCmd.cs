// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("PartitionCmd")]
    internal class PartitionCmd : Node
    {
        public RangeVar name { get; set; }

        public PartitionBoundSpec bound { get; set; }
    }
}
