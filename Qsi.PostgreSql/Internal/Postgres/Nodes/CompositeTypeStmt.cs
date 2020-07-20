// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CompositeTypeStmt")]
    internal class CompositeTypeStmt : Node
    {
        public RangeVar typevar { get; set; }

        public IPgTree[] coldeflist { get; set; }
    }
}
