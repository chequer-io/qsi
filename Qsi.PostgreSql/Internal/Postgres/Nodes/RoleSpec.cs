// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RoleSpec")]
    internal class RoleSpec : IPgTree
    {
        public RoleSpecType roletype { get; set; }

        public string rolename { get; set; }

        public int location { get; set; }
    }
}
