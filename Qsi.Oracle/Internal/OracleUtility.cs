using System;
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

        public static bool IsCommentPlanHint(ReadOnlySpan<char> text)
        {
            if (text.StartsWith("/*") || text.StartsWith("--"))
            {
                ReadOnlySpan<char> span = text[2..];

                while (!span.IsEmpty && span[0] != '+')
                {
                    if (!char.IsWhiteSpace(span[0]))
                        return false;

                    span = span[1..];
                }

                return !span.IsEmpty && span[0] == '+';
            }

            return false;
        }
    }
}
