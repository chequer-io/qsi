// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateEnumStmt")]
    internal class CreateEnumStmt : IPgTree
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] vals { get; set; }
    }
}
