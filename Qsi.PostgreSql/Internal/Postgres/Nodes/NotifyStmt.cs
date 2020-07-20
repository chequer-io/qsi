// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("NotifyStmt")]
    internal class NotifyStmt : Node
    {
        public string conditionname { get; set; }

        public string payload { get; set; }
    }
}
