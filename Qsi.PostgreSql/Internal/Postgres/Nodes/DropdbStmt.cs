// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropdbStmt")]
    internal class DropdbStmt : Node
    {
        public string dbname { get; set; }

        public bool missing_ok { get; set; }

        public IPgTree[] options { get; set; }
    }
}
