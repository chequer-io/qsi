// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CoalesceExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ coalescetype { get; set; }

        public string /* oid */ coalescecollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
