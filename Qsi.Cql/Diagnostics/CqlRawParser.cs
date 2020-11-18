using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Cassandra.Internal;
using Qsi.Diagnostics.Antlr;

namespace Qsi.Cassandra.Diagnostics
{
    public sealed class CqlRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new CqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Internal.CqlParser(tokens);

            return (parser.cqls(), parser.RuleNames);
        }
    }
}
