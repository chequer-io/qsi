using System.IO;
using Antlr4.Runtime;

namespace Qsi.Parsing.Antlr
{
    internal class UpperInputStream : AntlrInputStream
    {
        public UpperInputStream()
        {
        }

        public UpperInputStream(string input) : base(input)
        {
        }

        public UpperInputStream(char[] data, int numberOfActualCharsInArray) : base(data, numberOfActualCharsInArray)
        {
        }

        public UpperInputStream(TextReader r) : base(r)
        {
        }

        public UpperInputStream(TextReader r, int initialSize) : base(r, initialSize)
        {
        }

        public UpperInputStream(TextReader r, int initialSize, int readChunkSize) : base(r, initialSize, readChunkSize)
        {
        }

        public UpperInputStream(Stream input) : base(input)
        {
        }

        public UpperInputStream(Stream input, int initialSize) : base(input, initialSize)
        {
        }

        public UpperInputStream(Stream input, int initialSize, int readChunkSize) : base(input, initialSize, readChunkSize)
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
