// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SubLink
    {
        public Expr xpr { get; set; }

        public SubLinkType subLinkType { get; set; }

        public int subLinkId { get; set; }

        public IPgTree testexpr { get; set; }

        public IPgTree[] operName { get; set; }

        public IPgTree subselect { get; set; }

        public int location { get; set; }
    }
}
