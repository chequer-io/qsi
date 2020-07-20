// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum SubLinkType
    {
        EXISTS_SUBLINK,
        ALL_SUBLINK,
        ANY_SUBLINK,
        ROWCOMPARE_SUBLINK,
        EXPR_SUBLINK,
        MULTIEXPR_SUBLINK,
        ARRAY_SUBLINK,
        CTE_SUBLINK
    }
}
