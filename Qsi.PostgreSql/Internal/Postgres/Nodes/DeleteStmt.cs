// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DeleteStmt")]
    internal class DeleteStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public IPgTree[] usingClause { get; set; }

        public IPgTree whereClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }
    }
}
