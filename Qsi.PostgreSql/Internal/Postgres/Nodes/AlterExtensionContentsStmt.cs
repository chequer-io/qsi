// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterExtensionContentsStmt")]
    internal class AlterExtensionContentsStmt : IPgTree
    {
        public string extname { get; set; }

        public int action { get; set; }

        public ObjectType objtype { get; set; }

        public IPgTree @object { get; set; }
    }
}
