// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class BoolExpr
    {
        public Expr xpr { get; set; }

        public BoolExprType boolop { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
