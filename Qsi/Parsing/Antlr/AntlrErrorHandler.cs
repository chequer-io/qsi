using System;
using System.IO;
using Antlr4.Runtime;

namespace Qsi.Parsing.Antlr
{
    internal sealed class AntlrErrorHandler : IAntlrErrorListener<IToken>
    {
        public event EventHandler<QsiSyntaxErrorException> Error;

        void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int column, string msg, RecognitionException e)
        {
            Error?.Invoke(this, new QsiSyntaxErrorException(line, column, msg));
        }
    }
}
