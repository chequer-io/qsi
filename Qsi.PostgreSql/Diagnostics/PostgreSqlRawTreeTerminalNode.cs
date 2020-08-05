using Qsi.Diagnostics;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal PostgreSqlRawTreeTerminalNode(object value)
        {
            DisplayName = value.ToString();
        }
    }
}
