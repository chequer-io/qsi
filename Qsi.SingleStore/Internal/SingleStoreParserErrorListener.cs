using Antlr4.Runtime;
using Qsi.Parsing;

namespace Qsi.SingleStore.Internal;

internal sealed class SingleStoreParserErrorListener : IAntlrErrorListener<IToken>
{
    void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new QsiSyntaxErrorException(line, charPositionInLine, msg);
    }
}
