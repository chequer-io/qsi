// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeSubselect")]
    internal class RangeSubselect : IPgTree
    {
        public bool lateral { get; set; }

        public IPgTree subquery { get; set; }

        public Alias alias { get; set; }
    }
}
