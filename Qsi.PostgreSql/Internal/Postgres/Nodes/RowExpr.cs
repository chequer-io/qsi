// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class RowExpr
    {
        public Expr xpr { get; set; }

        public IPgTree[] args { get; set; }

        public int /* oid */ row_typeid { get; set; }

        public CoercionForm row_format { get; set; }

        public IPgTree[] colnames { get; set; }

        public int location { get; set; }
    }
}
