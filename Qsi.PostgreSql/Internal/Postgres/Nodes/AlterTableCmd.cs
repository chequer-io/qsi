// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterTableCmd")]
    internal class AlterTableCmd : IPgTree
    {
        public AlterTableType subtype { get; set; }

        public string name { get; set; }

        public short num { get; set; }

        public RoleSpec newowner { get; set; }

        public IPgTree def { get; set; }

        public DropBehavior behavior { get; set; }

        public bool missing_ok { get; set; }
    }
}
