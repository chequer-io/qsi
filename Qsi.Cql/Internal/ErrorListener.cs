using System.IO;
using Antlr4.Runtime;
using Qsi.Parsing;

namespace Qsi.Cql.Internal;

internal class ErrorListener : IAntlrErrorListener<IToken>
{
    void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int column, string msg, RecognitionException e)
    {
        throw new QsiSyntaxErrorException(line, column, msg);
    }

    public void SyntaxError(CqlParserInternal parser, string msg)
    {
        int line = -1;
        int column = -1;

        if (parser.CurrentToken != null)
        {
            line = parser.CurrentToken.Line;
            column = parser.CurrentToken.Column;
        }

        throw new QsiSyntaxErrorException(line, column, msg);
    }
}