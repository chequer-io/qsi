// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("A_ArrayExpr")]
    internal class A_ArrayExpr : IPgTree
    {
        public IPgTree[] elements { get; set; }

        public int location { get; set; }
    }
}
