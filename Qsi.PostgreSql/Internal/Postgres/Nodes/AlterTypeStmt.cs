// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterTypeStmt")]
    internal class AlterTypeStmt : IPgTree
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] options { get; set; }
    }
}
