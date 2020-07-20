// Generate from postgres/src/include/nodes/value.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("Value")]
    internal class Value : Node
    {
        public ValUnion val { get; set; }
    }
}
