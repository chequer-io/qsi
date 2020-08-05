// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ResTarget")]
    internal class ResTarget : IPgTree
    {
        public string name { get; set; }

        public IPgTree[] indirection { get; set; }

        public IPgTree val { get; set; }

        public int location { get; set; }
    }
}
