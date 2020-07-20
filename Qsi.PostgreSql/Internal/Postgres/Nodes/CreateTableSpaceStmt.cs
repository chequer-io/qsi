// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateTableSpaceStmt")]
    internal class CreateTableSpaceStmt : Node
    {
        public string tablespacename { get; set; }

        public RoleSpec owner { get; set; }

        public string location { get; set; }

        public IPgTree[] options { get; set; }
    }
}
