using Qsi.Diagnostics;

namespace Qsi.PhoenixSql.Diagnostics;

internal sealed class PhoenixSqlRawTerminalNode : IRawTreeTerminalNode 
{
    public string DisplayName { get; }

    public IRawTree[] Children { get; }

    public PhoenixSqlRawTerminalNode(object value)
    {
        DisplayName = value?.ToString() ?? "null";
        Children = null;
    }
}