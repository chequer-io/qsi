// Generate from postgres/src/include/nodes/nodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum JoinType
    {
        JOIN_INNER,
        JOIN_LEFT,
        JOIN_FULL,
        JOIN_RIGHT,
        JOIN_SEMI,
        JOIN_ANTI,
        JOIN_UNIQUE_OUTER,
        JOIN_UNIQUE_INNER
    }
}
