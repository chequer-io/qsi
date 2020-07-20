// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DropTableSpaceStmt")]
    internal class DropTableSpaceStmt : Node
    {
        public string tablespacename { get; set; }

        public bool missing_ok { get; set; }
    }
}
