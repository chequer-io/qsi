// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ListenStmt")]
    internal class ListenStmt : IPgTree
    {
        public string conditionname { get; set; }
    }
}
