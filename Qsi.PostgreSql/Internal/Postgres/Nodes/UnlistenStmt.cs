// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("UnlistenStmt")]
    internal class UnlistenStmt : Node
    {
        public string conditionname { get; set; }
    }
}
