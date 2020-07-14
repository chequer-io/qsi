using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.MySql.Internal;
using Qsi.Parsing.Antlr;

namespace Qsi.MySql.Diagnostics
{
    public class MySqlRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrUpperInputStream(input);
            var lexer = new MySqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Internal.MySqlParser(tokens);

            return (parser.root(), parser.RuleNames);
        }
    }
}
