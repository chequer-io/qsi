// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SubscriptingRef
    {
        public Expr xpr { get; set; }

        public string /* oid */ refcontainertype { get; set; }

        public string /* oid */ refelemtype { get; set; }

        public int reftypmod { get; set; }

        public string /* oid */ refcollid { get; set; }

        public IPgTree[] refupperindexpr { get; set; }

        public IPgTree[] reflowerindexpr { get; set; }

        public Expr refexpr { get; set; }

        public Expr refassgnexpr { get; set; }
    }
}
