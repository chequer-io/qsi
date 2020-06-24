using System;

namespace Qsi.Parsing
{
    public sealed class QsiSyntaxErrorException : Exception
    {
        public int Line { get; }

        public int Column { get; }

        public QsiSyntaxErrorException(int line, int column, string message) : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}
