// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ClosePortalStmt")]
    internal class ClosePortalStmt : IPgTree
    {
        public string portalname { get; set; }
    }
}
