// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum FunctionParameterMode
    {
        FUNC_PARAM_IN = 'i',
        FUNC_PARAM_OUT = 'o',
        FUNC_PARAM_INOUT = 'b',
        FUNC_PARAM_VARIADIC = 'v',
        FUNC_PARAM_TABLE = 't'
    }
}
