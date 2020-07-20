// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeSubselect")]
    internal class RangeSubselect : Node
    {
        public bool lateral { get; set; }

        public Node subquery { get; set; }

        public Alias alias { get; set; }
    }
}
