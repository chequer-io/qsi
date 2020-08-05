// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AccessPriv")]
    internal class AccessPriv : IPgTree
    {
        public string priv_name { get; set; }

        public IPgTree[] cols { get; set; }
    }
}
