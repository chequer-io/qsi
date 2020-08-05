// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CompositeTypeStmt")]
    internal class CompositeTypeStmt : IPgTree
    {
        public RangeVar typevar { get; set; }

        public IPgTree[] coldeflist { get; set; }
    }
}
