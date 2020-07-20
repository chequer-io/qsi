// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterForeignServerStmt")]
    internal class AlterForeignServerStmt : Node
    {
        public char servername { get; set; }

        public char version { get; set; }

        public IPgTree[] options { get; set; }

        public bool has_version { get; set; }
    }
}
