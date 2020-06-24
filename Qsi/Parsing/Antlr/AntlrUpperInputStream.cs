using System.IO;
using Antlr4.Runtime;

namespace Qsi.Parsing.Antlr
{
    public class AntlrUpperInputStream : AntlrInputStream
    {
        public AntlrUpperInputStream()
        {
        }

        public AntlrUpperInputStream(string input) : base(input)
        {
        }

        public AntlrUpperInputStream(char[] data, int numberOfActualCharsInArray) : base(data, numberOfActualCharsInArray)
        {
        }

        public AntlrUpperInputStream(TextReader r) : base(r)
        {
        }

        public AntlrUpperInputStream(TextReader r, int initialSize) : base(r, initialSize)
        {
        }

        public AntlrUpperInputStream(TextReader r, int initialSize, int readChunkSize) : base(r, initialSize, readChunkSize)
        {
        }

        public AntlrUpperInputStream(Stream input) : base(input)
        {
        }

        public AntlrUpperInputStream(Stream input, int initialSize) : base(input, initialSize)
        {
        }

        public AntlrUpperInputStream(Stream input, int initialSize, int readChunkSize) : base(input, initialSize, readChunkSize)
        {
        }

        public override int LA(int i)
        {
            int c = base.LA(i);

            if (c <= 0)
                return c;

            return char.ToUpperInvariant((char)c);
        }
    }
}
