// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CollateClause")]
    internal class CollateClause : Node
    {
        public Node arg { get; set; }

        public IPgTree[] collname { get; set; }

        public int location { get; set; }
    }
}
