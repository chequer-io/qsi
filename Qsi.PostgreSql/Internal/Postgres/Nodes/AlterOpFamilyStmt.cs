// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterOpFamilyStmt")]
    internal class AlterOpFamilyStmt : IPgTree
    {
        public IPgTree[] opfamilyname { get; set; }

        public string amname { get; set; }

        public bool isDrop { get; set; }

        public IPgTree[] items { get; set; }
    }
}
