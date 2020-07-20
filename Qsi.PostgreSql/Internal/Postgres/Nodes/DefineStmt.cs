// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("DefineStmt")]
    internal class DefineStmt : Node
    {
        public ObjectType kind { get; set; }

        public bool oldstyle { get; set; }

        public IPgTree[] defnames { get; set; }

        public IPgTree[] args { get; set; }

        public IPgTree[] definition { get; set; }

        public bool if_not_exists { get; set; }

        public bool replace { get; set; }
    }
}
