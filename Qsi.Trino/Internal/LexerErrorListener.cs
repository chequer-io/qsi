using System.IO;
using Antlr4.Runtime;

namespace Qsi.Trino.Internal
{
    internal sealed class LexerErrorListener : IAntlrErrorListener<int>
    {
        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ParsingException(msg, line, charPositionInLine + 1);
        }
    }
}
