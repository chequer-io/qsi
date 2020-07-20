// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CallContext")]
    internal class CallContext : Node
    {
        public bool atomic { get; set; }
    }
}
