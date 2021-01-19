using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Qsi.Parsing;

namespace Qsi.MySql.Internal
{
    internal class MySqlLexerErrorHandler : IAntlrErrorListener<int>
    {
        // modules/db.mysql.parser/src/mysql_parser_module.cpp
        // LexerErrorListener::syntaxError
        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int column, string msg, RecognitionException e)
        {
            if (e is LexerNoViableAltException)
            {
                var lexer = (Lexer)recognizer;
                var input = (ICharStream)lexer.InputStream;
                var text = lexer.GetErrorDisplay(input.GetText(new Interval(lexer.TokenStartCharIndex, input.Index)));

                if (string.IsNullOrEmpty(text))
                    text = " "; // Should never happen.

                switch (text[0])
                {
                    case '/':
                        msg = "Unfinished multiline comment";
                        break;

                    case '"':
                        msg = "Unfinished double quoted string literal";
                        break;

                    case '\'':
                        msg = "Unfinished single quoted string literal";
                        break;

                    case '`':
                        msg = "Unfinished back tick quoted string literal";
                        break;

                    default:
                        // Hex or bin string?
                        if (text.Length > 1 && text[1] == '\'' && (text[0] == 'x' || text[0] == 'b'))
                        {
                            msg = $"Unfinished {(text[0] == 'x' ? "hex" : "binary")} string literal";
                            break;
                        }

                        // Something else the lexer couldn't make sense of (likely there is no rule that accepts this input).
                        msg = $"\"{text}\" is no valid input at all";
                        break;
                }
            }

            throw new QsiSyntaxErrorException(line, column, msg);
        }
    }
}
