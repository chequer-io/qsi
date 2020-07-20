// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class NextValueExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ seqid { get; set; }

        public string /* oid */ typeId { get; set; }
    }
}
