using System;
using Qsi.Diagnostics;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string TypeName { get; }

        public string DisplayName { get; }

        public IRawTree[] Children => Array.Empty<IRawTree>();

        internal PostgreSqlRawTreeTerminalNode(string name, object? value)
        {
            TypeName = name;
            DisplayName = value?.ToString() ?? string.Empty;
        }

        internal PostgreSqlRawTreeTerminalNode(object? value)
        {
            TypeName = string.Empty;
            DisplayName = value?.ToString() ?? string.Empty;
        }
    }
}
