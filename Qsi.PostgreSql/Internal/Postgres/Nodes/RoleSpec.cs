// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RoleSpec")]
    internal class RoleSpec : Node
    {
        public RoleSpecType roletype { get; set; }

        public char rolename { get; set; }

        public int location { get; set; }
    }
}
