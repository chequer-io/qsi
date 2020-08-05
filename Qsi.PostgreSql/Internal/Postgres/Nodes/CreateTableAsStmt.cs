// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateTableAsStmt")]
    internal class CreateTableAsStmt : IPgTree
    {
        public IPgTree query { get; set; }

        public IntoClause into { get; set; }

        public ObjectType objtype { get; set; }

        public bool is_select_into { get; set; }

        public bool if_not_exists { get; set; }
    }
}
