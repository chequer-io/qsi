// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class RowCompareExpr
    {
        public Expr xpr { get; set; }

        public RowCompareType rctype { get; set; }

        public IPgTree[] opnos { get; set; }

        public IPgTree[] opfamilies { get; set; }

        public IPgTree[] inputcollids { get; set; }

        public IPgTree[] largs { get; set; }

        public IPgTree[] rargs { get; set; }
    }
}
