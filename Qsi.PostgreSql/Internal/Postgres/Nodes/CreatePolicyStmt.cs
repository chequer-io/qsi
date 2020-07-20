// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreatePolicyStmt")]
    internal class CreatePolicyStmt : Node
    {
        public string policy_name { get; set; }

        public RangeVar table { get; set; }

        public string cmd_name { get; set; }

        public bool permissive { get; set; }

        public IPgTree[] roles { get; set; }

        public Node qual { get; set; }

        public Node with_check { get; set; }
    }
}
