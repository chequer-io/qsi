// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("FromExpr")]
    internal class FromExpr : Node
    {
        public IPgTree[] fromlist { get; set; }

        public Node quals { get; set; }
    }
}
