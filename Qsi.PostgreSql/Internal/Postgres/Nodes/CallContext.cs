// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CallContext")]
    internal class CallContext : IPgTree
    {
        public bool atomic { get; set; }
    }
}
