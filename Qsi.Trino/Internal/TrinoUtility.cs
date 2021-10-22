using Antlr4.Runtime;

namespace Qsi.Trino.Internal
{
    internal class TrinoUtility
    {
        public static SqlBaseParser CreateParser(string input)
        {
            var stream = new AntlrInputStream(input);
            var lexer = new SqlBaseLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SqlBaseParser(tokens);

            parser.AddErrorListener(new ErrorListener());

            return parser;
        }
    }
}
