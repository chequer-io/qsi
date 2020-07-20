// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateSchemaStmt")]
    internal class CreateSchemaStmt : Node
    {
        public string schemaname { get; set; }

        public RoleSpec authrole { get; set; }

        public IPgTree[] schemaElts { get; set; }

        public bool if_not_exists { get; set; }
    }
}
