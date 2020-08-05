// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CopyStmt")]
    internal class CopyStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public IPgTree query { get; set; }

        public IPgTree[] attlist { get; set; }

        public bool is_from { get; set; }

        public bool is_program { get; set; }

        public string filename { get; set; }

        public IPgTree[] options { get; set; }

        public IPgTree whereClause { get; set; }
    }
}
