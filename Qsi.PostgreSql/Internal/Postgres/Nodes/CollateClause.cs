// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CollateClause")]
    internal class CollateClause : IPgTree
    {
        public IPgTree arg { get; set; }

        public IPgTree[] collname { get; set; }

        public int location { get; set; }
    }
}
