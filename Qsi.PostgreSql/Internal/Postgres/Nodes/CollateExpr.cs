// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CollateExpr
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public int /* oid */ collOid { get; set; }

        public int location { get; set; }
    }
}
