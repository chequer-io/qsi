// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterOperatorStmt")]
    internal class AlterOperatorStmt : IPgTree
    {
        public ObjectWithArgs opername { get; set; }

        public IPgTree[] options { get; set; }
    }
}
