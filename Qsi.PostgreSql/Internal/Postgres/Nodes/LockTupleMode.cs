// Generate from postgres/src/include/nodes/lockoptions.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum LockTupleMode
    {
        LockTupleKeyShare,
        LockTupleShare,
        LockTupleNoKeyExclusive,
        LockTupleExclusive
    }
}
