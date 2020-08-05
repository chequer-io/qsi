// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("A_Indirection")]
    internal class A_Indirection : IPgTree
    {
        public IPgTree arg { get; set; }

        public IPgTree[] indirection { get; set; }
    }
}
