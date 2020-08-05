// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("Alias")]
    internal class Alias : IPgTree
    {
        public string aliasname { get; set; }

        public IPgTree[] colnames { get; set; }
    }
}
