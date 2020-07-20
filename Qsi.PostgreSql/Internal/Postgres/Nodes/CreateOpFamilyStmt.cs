// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateOpFamilyStmt")]
    internal class CreateOpFamilyStmt : Node
    {
        public IPgTree[] opfamilyname { get; set; }

        public string amname { get; set; }
    }
}
