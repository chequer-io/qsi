// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CoalesceExpr
    {
        public Expr xpr { get; set; }

        public int /* oid */ coalescetype { get; set; }

        public int /* oid */ coalescecollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
