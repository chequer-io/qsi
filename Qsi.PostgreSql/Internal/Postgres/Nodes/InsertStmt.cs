// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("InsertStmt")]
    internal class InsertStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public IPgTree[] cols { get; set; }

        public IPgTree selectStmt { get; set; }

        public OnConflictClause onConflictClause { get; set; }

        public IPgTree[] returningList { get; set; }

        public WithClause withClause { get; set; }

        public OverridingKind @override { get; set; }
    }
}
