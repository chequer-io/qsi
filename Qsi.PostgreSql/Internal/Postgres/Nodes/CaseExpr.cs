// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CaseExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ casetype { get; set; }

        public string /* oid */ casecollid { get; set; }

        public Expr arg { get; set; }

        public IPgTree[] args { get; set; }

        public Expr defresult { get; set; }

        public int location { get; set; }
    }
}
