// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("Query")]
    internal class Query : Node
    {
        public CmdType commandType { get; set; }

        public QuerySource querySource { get; set; }

        public ulong queryId { get; set; }

        public bool canSetTag { get; set; }

        public Node utilityStmt { get; set; }

        public int resultRelation { get; set; }

        public bool hasAggs { get; set; }

        public bool hasWindowFuncs { get; set; }

        public bool hasTargetSRFs { get; set; }

        public bool hasSubLinks { get; set; }

        public bool hasDistinctOn { get; set; }

        public bool hasRecursive { get; set; }

        public bool hasModifyingCTE { get; set; }

        public bool hasForUpdate { get; set; }

        public bool hasRowSecurity { get; set; }

        public IPgTree[] cteList { get; set; }

        public IPgTree[] rtable { get; set; }

        public FromExpr jointree { get; set; }

        public IPgTree[] targetList { get; set; }

        public OverridingKind @override { get; set; }

        public OnConflictExpr onConflict { get; set; }

        public IPgTree[] returningList { get; set; }

        public IPgTree[] groupClause { get; set; }

        public IPgTree[] groupingSets { get; set; }

        public Node havingQual { get; set; }

        public IPgTree[] windowClause { get; set; }

        public IPgTree[] distinctClause { get; set; }

        public IPgTree[] sortClause { get; set; }

        public Node limitOffset { get; set; }

        public Node limitCount { get; set; }

        public LimitOption limitOption { get; set; }

        public IPgTree[] rowMarks { get; set; }

        public Node setOperations { get; set; }

        public IPgTree[] constraintDeps { get; set; }

        public IPgTree[] withCheckOptions { get; set; }

        public int stmt_location { get; set; }

        public int stmt_len { get; set; }
    }
}
