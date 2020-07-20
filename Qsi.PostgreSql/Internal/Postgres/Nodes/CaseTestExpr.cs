// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CaseTestExpr
    {
        public Expr xpr { get; set; }

        public int /* oid */ typeId { get; set; }

        public int typeMod { get; set; }

        public int /* oid */ collation { get; set; }
    }
}
