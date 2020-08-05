// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterSeqStmt")]
    internal class AlterSeqStmt : IPgTree
    {
        public RangeVar sequence { get; set; }

        public IPgTree[] options { get; set; }

        public bool for_identity { get; set; }

        public bool missing_ok { get; set; }
    }
}
