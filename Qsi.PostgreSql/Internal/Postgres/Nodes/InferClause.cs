// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("InferClause")]
    internal class InferClause : Node
    {
        public IPgTree[] indexElems { get; set; }

        public Node whereClause { get; set; }

        public char conname { get; set; }

        public int location { get; set; }
    }
}
