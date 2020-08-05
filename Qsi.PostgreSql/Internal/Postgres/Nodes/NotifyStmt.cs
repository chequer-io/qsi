// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("NotifyStmt")]
    internal class NotifyStmt : IPgTree
    {
        public string conditionname { get; set; }

        public string payload { get; set; }
    }
}
