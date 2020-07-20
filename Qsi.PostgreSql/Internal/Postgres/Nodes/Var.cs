// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class Var
    {
        public Expr xpr { get; set; }

        public index varno { get; set; }

        public short /* AttrNumber */ varattno { get; set; }

        public int /* oid */ vartype { get; set; }

        public int vartypmod { get; set; }

        public int /* oid */ varcollid { get; set; }

        public index varlevelsup { get; set; }

        public index varnosyn { get; set; }

        public short /* AttrNumber */ varattnosyn { get; set; }

        public int location { get; set; }
    }
}
