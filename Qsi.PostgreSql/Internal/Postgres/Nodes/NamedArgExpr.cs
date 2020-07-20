// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class NamedArgExpr
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public string name { get; set; }

        public int argnumber { get; set; }

        public int location { get; set; }
    }
}
