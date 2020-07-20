// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("VacuumStmt")]
    internal class VacuumStmt : Node
    {
        public IPgTree[] options { get; set; }

        public IPgTree[] rels { get; set; }

        public bool is_vacuumcmd { get; set; }
    }
}
