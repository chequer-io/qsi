// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterPolicyStmt")]
    internal class AlterPolicyStmt : IPgTree
    {
        public string policy_name { get; set; }

        public RangeVar table { get; set; }

        public IPgTree[] roles { get; set; }

        public IPgTree qual { get; set; }

        public IPgTree with_check { get; set; }
    }
}
