// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("A_Expr")]
    internal class A_Expr : Node
    {
        public A_Expr_Kind kind { get; set; }

        public IPgTree[] name { get; set; }

        public Node lexpr { get; set; }

        public Node rexpr { get; set; }

        public int location { get; set; }
    }
}
