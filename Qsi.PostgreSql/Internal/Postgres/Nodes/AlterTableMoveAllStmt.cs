// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTableMoveAllStmt")]
    internal class AlterTableMoveAllStmt : Node
    {
        public string orig_tablespacename { get; set; }

        public ObjectType objtype { get; set; }

        public IPgTree[] roles { get; set; }

        public string new_tablespacename { get; set; }

        public bool nowait { get; set; }
    }
}
