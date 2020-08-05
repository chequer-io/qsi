// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterOwnerStmt")]
    internal class AlterOwnerStmt : IPgTree
    {
        public ObjectType objectType { get; set; }

        public RangeVar relation { get; set; }

        public IPgTree @object { get; set; }

        public RoleSpec newowner { get; set; }
    }
}
