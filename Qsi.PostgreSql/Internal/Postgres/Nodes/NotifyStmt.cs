// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("NotifyStmt")]
    internal class NotifyStmt : Node
    {
        public char conditionname { get; set; }

        public char payload { get; set; }
    }
}
