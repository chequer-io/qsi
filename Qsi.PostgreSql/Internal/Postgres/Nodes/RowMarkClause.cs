// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RowMarkClause")]
    internal class RowMarkClause : IPgTree
    {
        public index rti { get; set; }

        public LockClauseStrength strength { get; set; }

        public LockWaitPolicy waitPolicy { get; set; }

        public bool pushedDown { get; set; }
    }
}
