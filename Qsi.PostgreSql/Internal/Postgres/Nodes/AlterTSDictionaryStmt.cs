// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterTSDictionaryStmt")]
    internal class AlterTSDictionaryStmt : IPgTree
    {
        public IPgTree[] dictname { get; set; }

        public IPgTree[] options { get; set; }
    }
}
