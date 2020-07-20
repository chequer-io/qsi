// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterOpFamilyStmt")]
    internal class AlterOpFamilyStmt : Node
    {
        public IPgTree[] opfamilyname { get; set; }

        public char amname { get; set; }

        public bool isDrop { get; set; }

        public IPgTree[] items { get; set; }
    }
}
