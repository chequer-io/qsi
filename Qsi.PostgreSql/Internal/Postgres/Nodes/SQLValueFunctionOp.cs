// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum SQLValueFunctionOp
    {
        SVFOP_CURRENT_DATE,
        SVFOP_CURRENT_TIME,
        SVFOP_CURRENT_TIME_N,
        SVFOP_CURRENT_TIMESTAMP,
        SVFOP_CURRENT_TIMESTAMP_N,
        SVFOP_LOCALTIME,
        SVFOP_LOCALTIME_N,
        SVFOP_LOCALTIMESTAMP,
        SVFOP_LOCALTIMESTAMP_N,
        SVFOP_CURRENT_ROLE,
        SVFOP_CURRENT_USER,
        SVFOP_USER,
        SVFOP_SESSION_USER,
        SVFOP_CURRENT_CATALOG,
        SVFOP_CURRENT_SCHEMA
    }
}
