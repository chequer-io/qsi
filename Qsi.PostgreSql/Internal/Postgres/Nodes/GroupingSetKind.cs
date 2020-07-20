// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum GroupingSetKind
    {
        GROUPING_SET_EMPTY,
        GROUPING_SET_SIMPLE,
        GROUPING_SET_ROLLUP,
        GROUPING_SET_CUBE,
        GROUPING_SET_SETS
    }
}
