// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateSchemaStmt")]
    internal class CreateSchemaStmt : IPgTree
    {
        public string schemaname { get; set; }

        public RoleSpec authrole { get; set; }

        public IPgTree[] schemaElts { get; set; }

        public bool if_not_exists { get; set; }
    }
}
