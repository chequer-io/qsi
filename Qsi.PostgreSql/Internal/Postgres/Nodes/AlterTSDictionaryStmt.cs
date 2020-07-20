// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTSDictionaryStmt")]
    internal class AlterTSDictionaryStmt : Node
    {
        public IPgTree[] dictname { get; set; }

        public IPgTree[] options { get; set; }
    }
}
