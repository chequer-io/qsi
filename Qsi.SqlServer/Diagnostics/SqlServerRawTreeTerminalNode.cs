using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    internal sealed class SqlServerRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal SqlServerRawTreeTerminalNode(object value)
        {
            DisplayName = value.ToString();
            Children = null;
        }
    }
}
