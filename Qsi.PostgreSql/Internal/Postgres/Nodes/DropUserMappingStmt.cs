// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropUserMappingStmt")]
    internal class DropUserMappingStmt : Node
    {
        public RoleSpec user { get; set; }

        public char servername { get; set; }

        public bool missing_ok { get; set; }
    }
}
