// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateRangeStmt")]
    internal class CreateRangeStmt : IPgTree
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] @params { get; set; }
    }
}
