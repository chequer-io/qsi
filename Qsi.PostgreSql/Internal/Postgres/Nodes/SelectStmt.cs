// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("SelectStmt")]
    internal class SelectStmt : IPgTree
    {
        public IPgTree[] distinctClause { get; set; }

        public IntoClause intoClause { get; set; }

        public IPgTree[] targetList { get; set; }

        public IPgTree[] fromClause { get; set; }

        public IPgTree whereClause { get; set; }

        public IPgTree[] groupClause { get; set; }

        public IPgTree havingClause { get; set; }

        public IPgTree[] windowClause { get; set; }

        public IPgTree[] valuesLists { get; set; }

        public IPgTree[] sortClause { get; set; }

        public IPgTree limitOffset { get; set; }

        public IPgTree limitCount { get; set; }

        public LimitOption limitOption { get; set; }

        public IPgTree[] lockingClause { get; set; }

        public WithClause withClause { get; set; }

        public SetOperation op { get; set; }

        public bool all { get; set; }

        public SelectStmt larg { get; set; }

        public SelectStmt rarg { get; set; }
    }
}
