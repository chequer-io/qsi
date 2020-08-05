// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("TableLikeClause")]
    internal class TableLikeClause : IPgTree
    {
        public RangeVar relation { get; set; }

        public uint /* bits32 */ options { get; set; }
    }
}
