// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("A_Const")]
    internal class A_Const : IPgTree
    {
        public PgValue val { get; set; }

        public int location { get; set; }
    }
}
