using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr
{
    public class AntlrRawTreeTerminalNode : IRawTreeTerminalNode
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        public AntlrRawTreeTerminalNode(ITerminalNode terminalNode)
        {
            DisplayName = terminalNode.ToString();
            Children = null;
        }
    }
}
