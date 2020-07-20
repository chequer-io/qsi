// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class ArrayExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ array_typeid { get; set; }

        public string /* oid */ array_collid { get; set; }

        public string /* oid */ element_typeid { get; set; }

        public IPgTree[] elements { get; set; }

        public bool multidims { get; set; }

        public int location { get; set; }
    }
}
