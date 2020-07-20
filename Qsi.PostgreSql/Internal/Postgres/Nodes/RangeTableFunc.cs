// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTableFunc")]
    internal class RangeTableFunc : Node
    {
        public bool lateral { get; set; }

        public Node docexpr { get; set; }

        public Node rowexpr { get; set; }

        public IPgTree[] namespaces { get; set; }

        public IPgTree[] columns { get; set; }

        public Alias alias { get; set; }

        public int location { get; set; }
    }
}
