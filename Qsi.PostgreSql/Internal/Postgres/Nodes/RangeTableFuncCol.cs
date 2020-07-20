// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTableFuncCol")]
    internal class RangeTableFuncCol : Node
    {
        public char colname { get; set; }

        public TypeName typeName { get; set; }

        public bool for_ordinality { get; set; }

        public bool is_not_null { get; set; }

        public Node colexpr { get; set; }

        public Node coldefexpr { get; set; }

        public int location { get; set; }
    }
}
