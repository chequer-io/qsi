// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ClosePortalStmt")]
    internal class ClosePortalStmt : Node
    {
        public string portalname { get; set; }
    }
}
