// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("UpdateStmt")]
    internal class UpdateStmt : Node
    {
        public RangeVar relation { get; set; }

        public IPgTree[] targetList { get; set; }

        public Node whereClause { get; set; }

        public IPgTree[] fromClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }
    }
}
