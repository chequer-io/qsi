// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RuleStmt")]
    internal class RuleStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public string rulename { get; set; }

        public IPgTree whereClause { get; set; }

        public CmdType @event { get; set; }

        public bool instead { get; set; }

        public IPgTree[] actions { get; set; }

        public bool replace { get; set; }
    }
}
