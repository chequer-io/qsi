// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterUserMappingStmt")]
    internal class AlterUserMappingStmt : IPgTree
    {
        public RoleSpec user { get; set; }

        public string servername { get; set; }

        public IPgTree[] options { get; set; }
    }
}
