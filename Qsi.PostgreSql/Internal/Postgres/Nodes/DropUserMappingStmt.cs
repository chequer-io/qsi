// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropUserMappingStmt")]
    internal class DropUserMappingStmt : IPgTree
    {
        public RoleSpec user { get; set; }

        public string servername { get; set; }

        public bool missing_ok { get; set; }
    }
}
