// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterDomainStmt")]
    internal class AlterDomainStmt : IPgTree
    {
        public char subtype { get; set; }

        public IPgTree[] typeName { get; set; }

        public string name { get; set; }

        public IPgTree def { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
