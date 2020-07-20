// Generate from postgres/src/include/nodes/lockoptions.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum LockClauseStrength
    {
        LCS_NONE,
        LCS_FORKEYSHARE,
        LCS_FORSHARE,
        LCS_FORNOKEYUPDATE,
        LCS_FORUPDATE
    }
}
