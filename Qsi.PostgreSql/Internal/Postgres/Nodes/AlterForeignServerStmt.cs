// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterForeignServerStmt")]
    internal class AlterForeignServerStmt : IPgTree
    {
        public string servername { get; set; }

        public string version { get; set; }

        public IPgTree[] options { get; set; }

        public bool has_version { get; set; }
    }
}
