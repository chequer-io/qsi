// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("MultiAssignRef")]
    internal class MultiAssignRef : Node
    {
        public Node source { get; set; }

        public int colno { get; set; }

        public int ncolumns { get; set; }
    }
}
