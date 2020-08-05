// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterExtensionStmt")]
    internal class AlterExtensionStmt : IPgTree
    {
        public string extname { get; set; }

        public IPgTree[] options { get; set; }
    }
}
