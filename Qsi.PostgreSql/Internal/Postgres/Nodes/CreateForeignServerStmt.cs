// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateForeignServerStmt")]
    internal class CreateForeignServerStmt : Node
    {
        public string servername { get; set; }

        public string servertype { get; set; }

        public string version { get; set; }

        public string fdwname { get; set; }

        public bool if_not_exists { get; set; }

        public IPgTree[] options { get; set; }
    }
}
