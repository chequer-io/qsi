// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("InsertStmt")]
    internal class InsertStmt : Node
    {
        public RangeVar relation { get; set; }

        public IPgTree[] cols { get; set; }

        public Node selectStmt { get; set; }

        public OnConflictClause onConflictClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }

        public OverridingKind @override { get; set; }
    }
}
