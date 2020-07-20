// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateForeignServerStmt")]
    internal class CreateForeignServerStmt : Node
    {
        public char servername { get; set; }

        public char servertype { get; set; }

        public char version { get; set; }

        public char fdwname { get; set; }

        public bool if_not_exists { get; set; }

        public IPgTree[] options { get; set; }
    }
}
