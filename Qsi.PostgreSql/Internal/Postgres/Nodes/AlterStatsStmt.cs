// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterStatsStmt")]
    internal class AlterStatsStmt : IPgTree
    {
        public IPgTree[] defnames { get; set; }

        public int stxstattarget { get; set; }

        public bool missing_ok { get; set; }
    }
}
