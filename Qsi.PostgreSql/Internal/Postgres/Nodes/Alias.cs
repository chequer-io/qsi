// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("Alias")]
    internal class Alias : Node
    {
        public string aliasname { get; set; }

        public IPgTree[] colnames { get; set; }
    }
}
