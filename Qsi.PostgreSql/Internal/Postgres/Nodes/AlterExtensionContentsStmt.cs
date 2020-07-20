// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterExtensionContentsStmt")]
    internal class AlterExtensionContentsStmt : Node
    {
        public char extname { get; set; }

        public int action { get; set; }

        public ObjectType objtype { get; set; }

        public Node @object { get; set; }
    }
}
