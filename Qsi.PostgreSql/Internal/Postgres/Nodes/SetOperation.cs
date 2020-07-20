// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum SetOperation
    {
        SETOP_NONE = 0,
        SETOP_UNION,
        SETOP_INTERSECT,
        SETOP_EXCEPT
    }
}
