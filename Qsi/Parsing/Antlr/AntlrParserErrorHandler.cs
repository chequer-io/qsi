using System.IO;
using Antlr4.Runtime;

namespace Qsi.Parsing.Antlr;

internal sealed class AntlrParserErrorHandler : IAntlrErrorListener<IToken>
{
    void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int column, string msg, RecognitionException e)
    {
        throw new QsiSyntaxErrorException(line, column, msg);
    }
}