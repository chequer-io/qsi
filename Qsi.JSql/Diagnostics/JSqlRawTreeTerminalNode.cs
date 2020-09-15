using net.sf.jsqlparser.parser;
using Qsi.Diagnostics;

namespace Qsi.JSql.Diagnostics
{
    internal sealed class JSqlRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; } = null;

        public JSqlRawTreeTerminalNode(Token token)
        {
            DisplayName = token.image;
        }

        public JSqlRawTreeTerminalNode(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
