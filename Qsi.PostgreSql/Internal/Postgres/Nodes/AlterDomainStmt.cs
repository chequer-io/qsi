// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterDomainStmt")]
    internal class AlterDomainStmt : Node
    {
        public char subtype { get; set; }

        public IPgTree[] typeName { get; set; }

        public string name { get; set; }

        public Node def { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
