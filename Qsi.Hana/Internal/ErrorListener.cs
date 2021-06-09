using System.IO;
using Antlr4.Runtime;
using Qsi.Parsing;

namespace Qsi.Hana.Internal
{
    internal class ErrorListener : IAntlrErrorListener<IToken>
    {
        void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int column, string msg, RecognitionException e)
        {
            throw new QsiSyntaxErrorException(line, column, msg);
        }
    }
}
