// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterCollationStmt")]
    internal class AlterCollationStmt : IPgTree
    {
        public IPgTree[] collname { get; set; }
    }
}
