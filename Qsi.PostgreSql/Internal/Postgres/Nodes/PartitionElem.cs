// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PartitionElem")]
    internal class PartitionElem : IPgTree
    {
        public string name { get; set; }

        public IPgTree expr { get; set; }

        public IPgTree[] collation { get; set; }

        public IPgTree[] opclass { get; set; }

        public int location { get; set; }
    }
}
