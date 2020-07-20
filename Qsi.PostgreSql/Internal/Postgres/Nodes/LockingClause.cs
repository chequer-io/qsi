// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("LockingClause")]
    internal class LockingClause : Node
    {
        public IPgTree[] lockedRels { get; set; }

        public LockClauseStrength strength { get; set; }

        public LockWaitPolicy waitPolicy { get; set; }
    }
}
