// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeTblRef")]
    internal class RangeTblRef : IPgTree
    {
        public int rtindex { get; set; }
    }
}
