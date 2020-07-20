// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateOpClassStmt")]
    internal class CreateOpClassStmt : Node
    {
        public IPgTree[] opclassname { get; set; }

        public IPgTree[] opfamilyname { get; set; }

        public char amname { get; set; }

        public TypeName datatype { get; set; }

        public IPgTree[] items { get; set; }

        public bool isDefault { get; set; }
    }
}
