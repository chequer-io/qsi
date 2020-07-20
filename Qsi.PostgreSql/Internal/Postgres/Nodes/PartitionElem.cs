// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("PartitionElem")]
    internal class PartitionElem : Node
    {
        public char name { get; set; }

        public Node expr { get; set; }

        public IPgTree[] collation { get; set; }

        public IPgTree[] opclass { get; set; }

        public int location { get; set; }
    }
}
