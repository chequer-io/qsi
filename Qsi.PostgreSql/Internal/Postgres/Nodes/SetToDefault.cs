// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SetToDefault
    {
        public Expr xpr { get; set; }

        public string /* oid */ typeId { get; set; }

        public int typeMod { get; set; }

        public string /* oid */ collation { get; set; }

        public int location { get; set; }
    }
}
