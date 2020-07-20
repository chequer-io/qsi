// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterCollationStmt")]
    internal class AlterCollationStmt : Node
    {
        public IPgTree[] collname { get; set; }
    }
}
