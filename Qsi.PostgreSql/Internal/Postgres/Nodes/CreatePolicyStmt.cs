// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreatePolicyStmt")]
    internal class CreatePolicyStmt : IPgTree
    {
        public string policy_name { get; set; }

        public RangeVar table { get; set; }

        public string cmd_name { get; set; }

        public bool permissive { get; set; }

        public IPgTree[] roles { get; set; }

        public IPgTree qual { get; set; }

        public IPgTree with_check { get; set; }
    }
}
