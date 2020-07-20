// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum RTEKind
    {
        RTE_RELATION,
        RTE_SUBQUERY,
        RTE_JOIN,
        RTE_FUNCTION,
        RTE_TABLEFUNC,
        RTE_VALUES,
        RTE_CTE,
        RTE_NAMEDTUPLESTORE,
        RTE_RESULT
    }
}
