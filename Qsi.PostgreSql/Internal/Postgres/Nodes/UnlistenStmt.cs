// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("UnlistenStmt")]
    internal class UnlistenStmt : IPgTree
    {
        public string conditionname { get; set; }
    }
}
