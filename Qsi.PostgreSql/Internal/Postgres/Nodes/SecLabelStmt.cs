// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("SecLabelStmt")]
    internal class SecLabelStmt : Node
    {
        public ObjectType objtype { get; set; }

        public Node @object { get; set; }

        public char provider { get; set; }

        public char label { get; set; }
    }
}
