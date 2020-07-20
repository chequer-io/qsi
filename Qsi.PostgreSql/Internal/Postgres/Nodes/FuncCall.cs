// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("FuncCall")]
    internal class FuncCall : Node
    {
        public IPgTree[] funcname { get; set; }

        public IPgTree[] args { get; set; }

        public IPgTree[] agg_order { get; set; }

        public Node agg_filter { get; set; }

        public bool agg_within_group { get; set; }

        public bool agg_star { get; set; }

        public bool agg_distinct { get; set; }

        public bool func_variadic { get; set; }

        public WindowDef over { get; set; }

        public int location { get; set; }
    }
}
