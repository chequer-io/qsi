// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PartitionBoundSpec")]
    internal class PartitionBoundSpec : IPgTree
    {
        public char strategy { get; set; }

        public bool is_default { get; set; }

        public int modulus { get; set; }

        public int remainder { get; set; }

        public IPgTree[] listdatums { get; set; }

        public IPgTree[] lowerdatums { get; set; }

        public IPgTree[] upperdatums { get; set; }

        public int location { get; set; }
    }
}
