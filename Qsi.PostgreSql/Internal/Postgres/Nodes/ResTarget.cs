// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ResTarget")]
    internal class ResTarget : Node
    {
        public string name { get; set; }

        public IPgTree[] indirection { get; set; }

        public Node val { get; set; }

        public int location { get; set; }
    }
}
