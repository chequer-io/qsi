// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateSeqStmt")]
    internal class CreateSeqStmt : Node
    {
        public RangeVar sequence { get; set; }

        public IPgTree[] options { get; set; }

        public int /* oid */ ownerId { get; set; }

        public bool for_identity { get; set; }

        public bool if_not_exists { get; set; }
    }
}
