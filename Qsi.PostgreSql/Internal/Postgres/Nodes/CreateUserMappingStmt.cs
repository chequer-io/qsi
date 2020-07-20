// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateUserMappingStmt")]
    internal class CreateUserMappingStmt : Node
    {
        public RoleSpec user { get; set; }

        public string servername { get; set; }

        public bool if_not_exists { get; set; }

        public IPgTree[] options { get; set; }
    }
}
