// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ParamRef")]
    internal class ParamRef : Node
    {
        public int number { get; set; }

        public int location { get; set; }
    }
}
