// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("IntoClause")]
    internal class IntoClause : Node
    {
        public RangeVar rel { get; set; }

        public IPgTree[] colNames { get; set; }

        public char accessMethod { get; set; }

        public IPgTree[] options { get; set; }

        public OnCommitAction onCommit { get; set; }

        public char tableSpaceName { get; set; }

        public Node viewQuery { get; set; }

        public bool skipData { get; set; }
    }
}
