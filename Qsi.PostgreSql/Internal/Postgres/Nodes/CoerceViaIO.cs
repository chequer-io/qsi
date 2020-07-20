// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CoerceViaIO
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public string /* oid */ resulttype { get; set; }

        public string /* oid */ resultcollid { get; set; }

        public CoercionForm coerceformat { get; set; }

        public int location { get; set; }
    }
}
