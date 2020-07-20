// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterForeignServerStmt")]
    internal class AlterForeignServerStmt : Node
    {
        public string servername { get; set; }

        public string version { get; set; }

        public IPgTree[] options { get; set; }

        public bool has_version { get; set; }
    }
}
