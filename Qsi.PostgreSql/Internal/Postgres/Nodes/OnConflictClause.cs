// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("OnConflictClause")]
    internal class OnConflictClause : IPgTree
    {
        public OnConflictAction action { get; set; }

        public InferClause infer { get; set; }

        public IPgTree[] targetList { get; set; }

        public IPgTree whereClause { get; set; }

        public int location { get; set; }
    }
}
