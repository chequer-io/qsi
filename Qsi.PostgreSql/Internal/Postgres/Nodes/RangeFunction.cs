// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeFunction")]
    internal class RangeFunction : Node
    {
        public bool lateral { get; set; }

        public bool ordinality { get; set; }

        public bool is_rowsfrom { get; set; }

        public IPgTree[] functions { get; set; }

        public Alias alias { get; set; }

        public IPgTree[] coldeflist { get; set; }
    }
}
