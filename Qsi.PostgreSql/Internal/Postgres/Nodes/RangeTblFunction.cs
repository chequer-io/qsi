// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTblFunction")]
    internal class RangeTblFunction : Node
    {
        public Node funcexpr { get; set; }

        public int funccolcount { get; set; }

        public IPgTree[] funccolnames { get; set; }

        public IPgTree[] funccoltypes { get; set; }

        public IPgTree[] funccoltypmods { get; set; }

        public IPgTree[] funccolcollations { get; set; }

        public Bitmapset funcparams { get; set; }
    }
}
