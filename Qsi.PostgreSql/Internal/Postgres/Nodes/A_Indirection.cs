// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("A_Indirection")]
    internal class A_Indirection : Node
    {
        public Node arg { get; set; }

        public IPgTree[] indirection { get; set; }
    }
}
