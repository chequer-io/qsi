// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("FromExpr")]
    internal class FromExpr : IPgTree
    {
        public IPgTree[] fromlist { get; set; }

        public IPgTree quals { get; set; }
    }
}
