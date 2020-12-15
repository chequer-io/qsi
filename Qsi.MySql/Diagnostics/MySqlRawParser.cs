using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.MySql.Internal;
using Qsi.Parsing.Antlr;

namespace Qsi.MySql.Diagnostics
{
    public class MySqlRawParser : AntlrRawParserBase
    {
        private readonly int _version;

        public MySqlRawParser(int version)
        {
            _version = version;
        }

        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var stream = new AntlrUpperInputStream(input);

            var lexer = new MySQLLexer(stream)
            {
                serverVersion = _version
            };

            var tokens = new CommonTokenStream(lexer);

            var parser = new MySQLParser(tokens)
            {
                serverVersion = _version
            };

            return (parser.query(), parser.RuleNames);
        }
    }
}
