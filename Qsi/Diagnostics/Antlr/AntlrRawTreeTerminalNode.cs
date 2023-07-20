using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr;

public class AntlrRawTreeTerminalNode : IRawTreeTerminalNode
{
    public string TypeName { get; }

    public string DisplayName { get; }

    public IRawTree[] Children { get; }

    public AntlrRawTreeTerminalNode(ITerminalNode terminalNode)
    {
        var symbol = terminalNode.Symbol;
        var recognizer = (IRecognizer)symbol?.TokenSource;

        if (recognizer == null)
        {
            DisplayName = terminalNode.ToString();
        }
        else
        {
            DisplayName = symbol.Text;
            TypeName = recognizer.Vocabulary.GetSymbolicName(symbol.Type);
        }

        Children = null;
    }
}