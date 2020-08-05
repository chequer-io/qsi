// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("GroupingSet")]
    internal class GroupingSet : IPgTree
    {
        public GroupingSetKind kind { get; set; }

        public IPgTree[] content { get; set; }

        public int location { get; set; }
    }
}
