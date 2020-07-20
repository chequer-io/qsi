// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CoerceToDomain
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public int /* oid */ resulttype { get; set; }

        public int resulttypmod { get; set; }

        public int /* oid */ resultcollid { get; set; }

        public CoercionForm coercionformat { get; set; }

        public int location { get; set; }
    }
}
