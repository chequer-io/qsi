// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("PartitionSpec")]
    internal class PartitionSpec : Node
    {
        public string strategy { get; set; }

        public IPgTree[] partParams { get; set; }

        public int location { get; set; }
    }
}
