// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TableFunc")]
    internal class TableFunc : Node
    {
        public IPgTree[] ns_uris { get; set; }

        public IPgTree[] ns_names { get; set; }

        public Node docexpr { get; set; }

        public Node rowexpr { get; set; }

        public IPgTree[] colnames { get; set; }

        public IPgTree[] coltypes { get; set; }

        public IPgTree[] coltypmods { get; set; }

        public IPgTree[] colcollations { get; set; }

        public IPgTree[] colexprs { get; set; }

        public IPgTree[] coldefexprs { get; set; }

        public Bitmapset notnulls { get; set; }

        public int ordinalitycol { get; set; }

        public int location { get; set; }
    }
}
