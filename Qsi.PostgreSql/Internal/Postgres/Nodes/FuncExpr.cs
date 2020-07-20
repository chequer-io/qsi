// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class FuncExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ funcid { get; set; }

        public string /* oid */ funcresulttype { get; set; }

        public bool funcretset { get; set; }

        public bool funcvariadic { get; set; }

        public CoercionForm funcformat { get; set; }

        public string /* oid */ funccollid { get; set; }

        public string /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
