// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum A_Expr_Kind
    {
        AEXPR_OP,
        AEXPR_OP_ANY,
        AEXPR_OP_ALL,
        AEXPR_DISTINCT,
        AEXPR_NOT_DISTINCT,
        AEXPR_NULLIF,
        AEXPR_OF,
        AEXPR_IN,
        AEXPR_LIKE,
        AEXPR_ILIKE,
        AEXPR_SIMILAR,
        AEXPR_BETWEEN,
        AEXPR_NOT_BETWEEN,
        AEXPR_BETWEEN_SYM,
        AEXPR_NOT_BETWEEN_SYM,
        AEXPR_PAREN
    }
}
