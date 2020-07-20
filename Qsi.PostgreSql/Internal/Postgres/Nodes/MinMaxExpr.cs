// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class MinMaxExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ minmaxtype { get; set; }

        public string /* oid */ minmaxcollid { get; set; }

        public string /* oid */ inputcollid { get; set; }

        public MinMaxOp op { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
