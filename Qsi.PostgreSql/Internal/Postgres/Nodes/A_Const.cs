// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("A_Const")]
    internal class A_Const : Node
    {
        public Value val { get; set; }

        public int location { get; set; }
    }
}
