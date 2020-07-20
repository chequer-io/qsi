// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum ConstrType
    {
        CONSTR_NULL,
        CONSTR_NOTNULL,
        CONSTR_DEFAULT,
        CONSTR_IDENTITY,
        CONSTR_GENERATED,
        CONSTR_CHECK,
        CONSTR_PRIMARY,
        CONSTR_UNIQUE,
        CONSTR_EXCLUSION,
        CONSTR_FOREIGN,
        CONSTR_ATTR_DEFERRABLE,
        CONSTR_ATTR_NOT_DEFERRABLE,
        CONSTR_ATTR_DEFERRED,
        CONSTR_ATTR_IMMEDIATE
    }
}
