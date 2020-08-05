// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateExtensionStmt")]
    internal class CreateExtensionStmt : IPgTree
    {
        public string extname { get; set; }

        public bool if_not_exists { get; set; }

        public IPgTree[] options { get; set; }
    }
}
