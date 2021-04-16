using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Hana.Internal;

namespace Qsi.Hana.Diagnostics
{
    public sealed class HanaRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new HanaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new HanaParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            return (parser.root(), parser.RuleNames);
        }
    }
}
