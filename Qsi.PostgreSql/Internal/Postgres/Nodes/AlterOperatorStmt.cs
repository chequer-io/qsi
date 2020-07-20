// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterOperatorStmt")]
    internal class AlterOperatorStmt : Node
    {
        public ObjectWithArgs opername { get; set; }

        public IPgTree[] options { get; set; }
    }
}
