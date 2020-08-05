// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("LockingClause")]
    internal class LockingClause : IPgTree
    {
        public IPgTree[] lockedRels { get; set; }

        public LockClauseStrength strength { get; set; }

        public LockWaitPolicy waitPolicy { get; set; }
    }
}
