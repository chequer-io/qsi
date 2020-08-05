// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateStatsStmt")]
    internal class CreateStatsStmt : IPgTree
    {
        public IPgTree[] defnames { get; set; }

        public IPgTree[] stat_types { get; set; }

        public IPgTree[] exprs { get; set; }

        public IPgTree[] relations { get; set; }

        public string stxcomment { get; set; }

        public bool if_not_exists { get; set; }
    }
}
