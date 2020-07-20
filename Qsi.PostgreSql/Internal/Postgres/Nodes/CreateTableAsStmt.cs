// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateTableAsStmt")]
    internal class CreateTableAsStmt : Node
    {
        public Node query { get; set; }

        public IntoClause into { get; set; }

        public ObjectType objtype { get; set; }

        public bool is_select_into { get; set; }

        public bool if_not_exists { get; set; }
    }
}
