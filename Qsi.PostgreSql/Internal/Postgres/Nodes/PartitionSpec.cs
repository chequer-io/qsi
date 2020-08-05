// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PartitionSpec")]
    internal class PartitionSpec : IPgTree
    {
        public string strategy { get; set; }

        public IPgTree[] partParams { get; set; }

        public int location { get; set; }
    }
}
