// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SubPlan
    {
        public Expr xpr { get; set; }

        public SubLinkType subLinkType { get; set; }

        public Node testexpr { get; set; }

        public IPgTree[] paramIds { get; set; }

        public int plan_id { get; set; }

        public char plan_name { get; set; }

        public string /* oid */ firstColType { get; set; }

        public int firstColTypmod { get; set; }

        public string /* oid */ firstColCollation { get; set; }

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
