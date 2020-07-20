// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterStatsStmt")]
    internal class AlterStatsStmt : Node
    {
        public IPgTree[] defnames { get; set; }

        public int stxstattarget { get; set; }

        public bool missing_ok { get; set; }
    }
}
