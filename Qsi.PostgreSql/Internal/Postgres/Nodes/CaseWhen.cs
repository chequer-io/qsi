// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CaseWhen
    {
        public Expr xpr { get; set; }

        public Expr expr { get; set; }

        public Expr result { get; set; }

        public int location { get; set; }
    }
}
