// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("UpdateStmt")]
    internal class UpdateStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public IPgTree[] targetList { get; set; }

        public IPgTree whereClause { get; set; }

        public IPgTree[] fromClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }
    }
}
