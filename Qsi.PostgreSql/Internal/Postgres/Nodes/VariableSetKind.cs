// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum VariableSetKind
    {
        VAR_SET_VALUE,
        VAR_SET_DEFAULT,
        VAR_SET_CURRENT,
        VAR_SET_MULTI,
        VAR_RESET,
        VAR_RESET_ALL
    }
}
