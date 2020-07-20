// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateExtensionStmt")]
    internal class CreateExtensionStmt : Node
    {
        public char extname { get; set; }

        public bool if_not_exists { get; set; }

        public IPgTree[] options { get; set; }
    }
}
