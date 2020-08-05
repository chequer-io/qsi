// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DropTableSpaceStmt")]
    internal class DropTableSpaceStmt : IPgTree
    {
        public string tablespacename { get; set; }

        public bool missing_ok { get; set; }
    }
}
