// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class ConvertRowtypeExpr
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public string /* oid */ resulttype { get; set; }

        public CoercionForm convertformat { get; set; }

        public int location { get; set; }
    }
}
