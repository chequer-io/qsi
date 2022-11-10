using Antlr4.Runtime;
using Antlr4.Runtime.Atn;

namespace Qsi.PostgreSql.Internal;

internal static class PostgreSqlUtility
{
    public static PostgreSqlParserInternal CreateParser(string input)
    {
        var stream = new AntlrInputStream(input);

        var lexer = new PostgreSqlLexerInternal(stream);
        
        // NOTE: Add custom error listener

        var tokens = new CommonTokenStream(lexer);

        var parser = new PostgreSqlParserInternal(tokens)
        {
            Interpreter =
            {
                PredictionMode = PredictionMode.SLL
            }
        };
        
        // NOTE: Add custom error listener

        return parser;
    }
}
