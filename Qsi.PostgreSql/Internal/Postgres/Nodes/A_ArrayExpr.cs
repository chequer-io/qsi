// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("A_ArrayExpr")]
    internal class A_ArrayExpr : Node
    {
        public IPgTree[] elements { get; set; }

        public int location { get; set; }
    }
}
