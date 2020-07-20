// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("A_Indices")]
    internal class A_Indices : Node
    {
        public bool is_slice { get; set; }

        public Node lidx { get; set; }

        public Node uidx { get; set; }
    }
}
