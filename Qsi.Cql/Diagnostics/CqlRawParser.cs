using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Cql.Internal;
using Qsi.Diagnostics.Antlr;

namespace Qsi.Cql.Diagnostics
{
    public sealed class CqlRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new CqlLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CqlParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            return (parser.root(), parser.RuleNames);
        }
    }
}
