// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterOwnerStmt")]
    internal class AlterOwnerStmt : Node
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public Node @object { get; set; }

        public RoleSpec newowner { get; set; }
    }
}
