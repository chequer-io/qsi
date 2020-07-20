// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateRangeStmt")]
    internal class CreateRangeStmt : Node
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] @params { get; set; }
    }
}
