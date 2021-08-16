using Antlr4.Runtime;

namespace Qsi.Oracle.Internal
{
    internal static class OracleUtility
    {
        public static OracleParserInternal CreateParser(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new OracleLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new OracleParserInternal(tokens);

            parser.AddErrorListener(new ErrorListener());

            return parser;
        }
    }
}
