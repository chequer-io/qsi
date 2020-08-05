// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterEventTrigStmt")]
    internal class AlterEventTrigStmt : IPgTree
    {
        public string trigname { get; set; }

        public char tgenabled { get; set; }
    }
}
