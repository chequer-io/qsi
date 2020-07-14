using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Parsing.Antlr;
using Qsi.PostgreSql.Internal;

namespace Qsi.PostgreSql.Diagnostics
{
    public class PostgreSqlRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrUpperInputStream(input);
            var lexer = new PlSqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PlSqlParser(tokens);

            return (parser.sql_script(), parser.RuleNames);
        }
    }
}
