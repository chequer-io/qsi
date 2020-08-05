// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateTableSpaceStmt")]
    internal class CreateTableSpaceStmt : IPgTree
    {
        public string tablespacename { get; set; }

        public RoleSpec owner { get; set; }

        public string location { get; set; }

        public IPgTree[] options { get; set; }
    }
}
