// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterUserMappingStmt")]
    internal class AlterUserMappingStmt : Node
    {
        public RoleSpec user { get; set; }

        public char servername { get; set; }

        public IPgTree[] options { get; set; }
    }
}
