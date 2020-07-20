// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CurrentOfExpr
    {
        public Expr xpr { get; set; }

        public index cvarno { get; set; }

        public char cursor_name { get; set; }

        public int cursor_param { get; set; }
    }
}
