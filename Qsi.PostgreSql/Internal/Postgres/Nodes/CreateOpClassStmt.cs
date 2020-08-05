// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateOpClassStmt")]
    internal class CreateOpClassStmt : IPgTree
    {
        public IPgTree[] opclassname { get; set; }

        public IPgTree[] opfamilyname { get; set; }

        public string amname { get; set; }

        public TypeName datatype { get; set; }

        public IPgTree[] items { get; set; }

        public bool isDefault { get; set; }
    }
}
