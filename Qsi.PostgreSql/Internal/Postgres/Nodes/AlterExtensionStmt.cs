// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterExtensionStmt")]
    internal class AlterExtensionStmt : Node
    {
        public char extname { get; set; }

        public IPgTree[] options { get; set; }
    }
}
