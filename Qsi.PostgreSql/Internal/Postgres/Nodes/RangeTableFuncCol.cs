// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeTableFuncCol")]
    internal class RangeTableFuncCol : IPgTree
    {
        public string colname { get; set; }

        public TypeName typeName { get; set; }

        public bool for_ordinality { get; set; }

        public bool is_not_null { get; set; }

        public IPgTree colexpr { get; set; }

        public IPgTree coldefexpr { get; set; }

        public int location { get; set; }
    }
}
