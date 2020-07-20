// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TableLikeClause")]
    internal class TableLikeClause : Node
    {
        public RangeVar relation { get; set; }

        public uint /* bits32 */ options { get; set; }
    }
}
