// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("GroupingSet")]
    internal class GroupingSet : Node
    {
        public GroupingSetKind kind { get; set; }

        public IPgTree[] content { get; set; }

        public int location { get; set; }
    }
}
