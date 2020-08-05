// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("WithClause")]
    internal class WithClause : IPgTree
    {
        public IPgTree[] ctes { get; set; }

        public bool recursive { get; set; }

        public int location { get; set; }
    }
}
