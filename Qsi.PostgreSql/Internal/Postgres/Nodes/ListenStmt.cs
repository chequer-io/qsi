// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ListenStmt")]
    internal class ListenStmt : Node
    {
        public char conditionname { get; set; }
    }
}
