// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateOpFamilyStmt")]
    internal class CreateOpFamilyStmt : IPgTree
    {
        public IPgTree[] opfamilyname { get; set; }

        public string amname { get; set; }
    }
}
