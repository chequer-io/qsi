// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateTableSpaceStmt")]
    internal class CreateTableSpaceStmt : Node
    {
        public char tablespacename { get; set; }

        public RoleSpec owner { get; set; }

        public char location { get; set; }

        public IPgTree[] options { get; set; }
    }
}
