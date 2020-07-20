// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTypeStmt")]
    internal class AlterTypeStmt : Node
    {
        public IPgTree[] typeName { get; set; }

        public IPgTree[] options { get; set; }
    }
}
