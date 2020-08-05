// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PartitionRangeDatum")]
    internal class PartitionRangeDatum : IPgTree
    {
        public PartitionRangeDatumKind kind { get; set; }

        public IPgTree value { get; set; }

        public int location { get; set; }
    }
}
