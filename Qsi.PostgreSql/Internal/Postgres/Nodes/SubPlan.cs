// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SubPlan
    {
        public Expr xpr { get; set; }

        public SubLinkType subLinkType { get; set; }

        public IPgTree testexpr { get; set; }

        public IPgTree[] paramIds { get; set; }

        public int plan_id { get; set; }

        public string plan_name { get; set; }

        public int /* oid */ firstColType { get; set; }

        public int firstColTypmod { get; set; }

        public int /* oid */ firstColCollation { get; set; }

        public bool useHashTable { get; set; }

        public bool unknownEqFalse { get; set; }

        public bool parallel_safe { get; set; }

        public IPgTree[] setParam { get; set; }

        public IPgTree[] parParam { get; set; }

        public IPgTree[] args { get; set; }

        public double /* Cost */ startup_cost { get; set; }

        public double /* Cost */ per_call_cost { get; set; }
    }
}
