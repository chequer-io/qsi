using Antlr4.Runtime;
using Qsi.Parsing;

namespace Qsi.SingleStore.Internal;

internal sealed class SingleStoreLexerErrorListener : IAntlrErrorListener<int>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new QsiSyntaxErrorException(line, charPositionInLine, msg);
    }
}
