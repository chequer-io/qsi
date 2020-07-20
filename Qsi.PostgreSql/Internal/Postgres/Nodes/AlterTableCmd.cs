// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTableCmd")]
    internal class AlterTableCmd : Node
    {
        public AlterTableType subtype { get; set; }

        public string name { get; set; }

        public short num { get; set; }

        public RoleSpec newowner { get; set; }

        public Node def { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
