// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateEnumStmt")]
    internal class CreateEnumStmt : Node
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] vals { get; set; }
    }
}
