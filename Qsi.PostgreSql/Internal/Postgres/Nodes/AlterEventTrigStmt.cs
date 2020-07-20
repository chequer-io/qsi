// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterEventTrigStmt")]
    internal class AlterEventTrigStmt : Node
    {
        public char trigname { get; set; }

        public char tgenabled { get; set; }
    }
}
