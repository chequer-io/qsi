// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RuleStmt")]
    internal class RuleStmt : Node
    {
        public RangeVar relation { get; set; }

        public char rulename { get; set; }

        public Node whereClause { get; set; }

        public CmdType @event { get; set; }

        public bool instead { get; set; }

        public IPgTree[] actions { get; set; }

        public bool replace { get; set; }
    }
}
