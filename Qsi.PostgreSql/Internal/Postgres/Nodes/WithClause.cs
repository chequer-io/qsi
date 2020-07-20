// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("WithClause")]
    internal class WithClause : Node
    {
        public IPgTree[] ctes { get; set; }

        public bool recursive { get; set; }

        public int location { get; set; }
    }
}
