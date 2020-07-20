// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TriggerTransition")]
    internal class TriggerTransition : Node
    {
        public string name { get; set; }

        public bool isNew { get; set; }

        public bool isTable { get; set; }
    }
}
