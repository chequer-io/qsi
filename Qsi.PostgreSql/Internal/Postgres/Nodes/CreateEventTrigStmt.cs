// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateEventTrigStmt")]
    internal class CreateEventTrigStmt : Node
    {
        public string trigname { get; set; }

        public string eventname { get; set; }

        public IPgTree[] whenclause { get; set; }

        public IPgTree[] funcname { get; set; }
    }
}
