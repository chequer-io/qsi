using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Impala.Internal;

namespace Qsi.Impala.Diagnostics
{
    public class ImpalaRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new ImpalaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ImpalaParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            return (parser.root(), parser.RuleNames);
        }
    }
}
