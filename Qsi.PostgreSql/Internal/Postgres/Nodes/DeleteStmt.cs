// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DeleteStmt")]
    internal class DeleteStmt : Node
    {
        public RangeVar relation { get; set; }

        public IPgTree[] usingClause { get; set; }

        public Node whereClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }
    }
}
