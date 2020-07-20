// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("OnConflictClause")]
    internal class OnConflictClause : Node
    {
        public OnConflictAction action { get; set; }

        public InferClause infer { get; set; }

        public IPgTree[] targetList { get; set; }

        public Node whereClause { get; set; }

        public int location { get; set; }
    }
}
