// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("InferClause")]
    internal class InferClause : IPgTree
    {
        public IPgTree[] indexElems { get; set; }

        public IPgTree whereClause { get; set; }

        public string conname { get; set; }

        public int location { get; set; }
    }
}
