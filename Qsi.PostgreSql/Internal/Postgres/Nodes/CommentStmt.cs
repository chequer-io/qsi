// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CommentStmt")]
    internal class CommentStmt : IPgTree
    {
        public ObjectType objtype { get; set; }

        public IPgTree @object { get; set; }

        public string comment { get; set; }
    }
}
