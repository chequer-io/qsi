// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeTblFunction")]
    internal class RangeTblFunction : IPgTree
    {
        public IPgTree funcexpr { get; set; }

        public int funccolcount { get; set; }

        public IPgTree[] funccolnames { get; set; }

        public IPgTree[] funccoltypes { get; set; }

        public IPgTree[] funccoltypmods { get; set; }

        public IPgTree[] funccolcollations { get; set; }

        public Bitmapset funcparams { get; set; }
    }
}
