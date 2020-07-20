// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TruncateStmt")]
    internal class TruncateStmt : Node
    {
        public IPgTree[] relations { get; set; }

        public bool restart_seqs { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
