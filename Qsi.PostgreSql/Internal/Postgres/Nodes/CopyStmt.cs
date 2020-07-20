// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CopyStmt")]
    internal class CopyStmt : Node
    {
        public RangeVar relation { get; set; }

        public Node query { get; set; }

        public IPgTree[] attlist { get; set; }

        public bool is_from { get; set; }

        public bool is_program { get; set; }

        public char filename { get; set; }

        public IPgTree[] options { get; set; }

        public Node whereClause { get; set; }
    }
}
