// Generate from postgres/src/include/nodes/nodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum CmdType
    {
        CMD_UNKNOWN,
        CMD_SELECT,
        CMD_UPDATE,
        CMD_INSERT,
        CMD_DELETE,
        CMD_UTILITY,
        CMD_NOTHING
    }
}
