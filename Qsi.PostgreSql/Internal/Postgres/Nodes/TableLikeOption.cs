// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum TableLikeOption
    {
        CREATE_TABLE_LIKE_COMMENTS = 1 << 0,
        CREATE_TABLE_LIKE_CONSTRAINTS = 1 << 1,
        CREATE_TABLE_LIKE_DEFAULTS = 1 << 2,
        CREATE_TABLE_LIKE_GENERATED = 1 << 3,
        CREATE_TABLE_LIKE_IDENTITY = 1 << 4,
        CREATE_TABLE_LIKE_INDEXES = 1 << 5,
        CREATE_TABLE_LIKE_STATISTICS = 1 << 6,
        CREATE_TABLE_LIKE_STORAGE = 1 << 7,
        /*
         * #define PG_INT32_MAX 0x7FFFFFFF
        */
        CREATE_TABLE_LIKE_ALL = 0x7FFFFFFF
    }
}
