// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class FuncExpr
    {
        public Expr xpr { get; set; }

        public int /* oid */ funcid { get; set; }

        public int /* oid */ funcresulttype { get; set; }

        public bool funcretset { get; set; }

        public bool funcvariadic { get; set; }

        public CoercionForm funcformat { get; set; }

        public int /* oid */ funccollid { get; set; }

        public int /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}
