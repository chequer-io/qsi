// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class NextValueExpr
    {
        public Expr xpr { get; set; }

        public int /* oid */ seqid { get; set; }

        public int /* oid */ typeId { get; set; }
    }
}
