// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTSConfigurationStmt")]
    internal class AlterTSConfigurationStmt : Node
    {
        public AlterTSConfigType kind { get; set; }

        public IPgTree[] cfgname { get; set; }

        public IPgTree[] tokentype { get; set; }

        public IPgTree[] dicts { get; set; }

        public bool @override { get; set; }

        public bool replace { get; set; }

        public bool missing_ok { get; set; }
    }
}
