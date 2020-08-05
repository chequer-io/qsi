// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterFunctionStmt")]
    internal class AlterFunctionStmt : IPgTree
    {
        public ObjectType objtype { get; set; }

        public ObjectWithArgs func { get; set; }

        public IPgTree[] actions { get; set; }
    }
}
