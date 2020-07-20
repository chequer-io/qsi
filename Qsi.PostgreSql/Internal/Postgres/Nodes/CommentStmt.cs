// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CommentStmt")]
    internal class CommentStmt : Node
    {
        public ObjectType objtype { get; set; }

        public Node @object { get; set; }

        public char comment { get; set; }
    }
}
