// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("PartitionRangeDatum")]
    internal class PartitionRangeDatum : Node
    {
        public PartitionRangeDatumKind kind { get; set; }

        public Node value { get; set; }

        public int location { get; set; }
    }
}
