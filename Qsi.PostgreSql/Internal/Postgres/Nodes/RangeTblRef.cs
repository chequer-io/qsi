// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTblRef")]
    internal class RangeTblRef : Node
    {
        public int rtindex { get; set; }
    }
}
