// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum OnCommitAction
    {
        ONCOMMIT_NOOP,
        ONCOMMIT_PRESERVE_ROWS,
        ONCOMMIT_DELETE_ROWS,
        ONCOMMIT_DROP
    }
}
