// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("GrantStmt")]
    internal class GrantStmt : Node
    {
        public bool is_grant { get; set; }

        public GrantTargetType targtype { get; set; }

        public ObjectType objtype { get; set; }

        public IPgTree[] objects { get; set; }

        public IPgTree[] privileges { get; set; }

        public IPgTree[] grantees { get; set; }

        public bool grant_option { get; set; }

        public DropBehavior behavior { get; set; }
    }
}
