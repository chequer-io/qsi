// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class GroupingFunc
    {
        public Expr xpr { get; set; }

        public IPgTree[] args { get; set; }

        public IPgTree[] refs { get; set; }

        public IPgTree[] cols { get; set; }

        public index agglevelsup { get; set; }

        public int location { get; set; }
    }
}
