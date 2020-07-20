// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("IndexStmt")]
    internal class IndexStmt : Node
    {
        public char idxname { get; set; }

        public RangeVar relation { get; set; }

        public char accessMethod { get; set; }

        public char tableSpace { get; set; }

        public IPgTree[] indexParams { get; set; }

        public IPgTree[] indexIncludingParams { get; set; }

        public IPgTree[] options { get; set; }

        public Node whereClause { get; set; }

        public IPgTree[] excludeOpNames { get; set; }

        public char idxcomment { get; set; }

        public string /* oid */ indexOid { get; set; }

        public string /* oid */ oldNode { get; set; }

        public uint /* SubTransactionId */ oldCreateSubid { get; set; }

        public uint /* SubTransactionId */ oldFirstRelfilenodeSubid { get; set; }

        public bool unique { get; set; }

        public bool primary { get; set; }

        public bool isconstraint { get; set; }

        public bool deferrable { get; set; }

        public bool initdeferred { get; set; }

        public bool transformed { get; set; }

        public bool concurrent { get; set; }

        public bool if_not_exists { get; set; }

        public bool reset_default_tblspc { get; set; }
    }
}
