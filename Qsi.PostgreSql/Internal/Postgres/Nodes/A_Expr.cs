// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("A_Expr")]
    internal class A_Expr : IPgTree
    {
        public A_Expr_Kind kind { get; set; }

        public IPgTree[] name { get; set; }

        public IPgTree lexpr { get; set; }

        public IPgTree rexpr { get; set; }

        public int location { get; set; }
    }
}
