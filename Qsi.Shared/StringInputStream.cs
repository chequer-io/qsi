using Antlr4.Runtime;

namespace Qsi.Shared
{
    internal class StringInputStream : AntlrInputStream
    {
        public string Input { get; }

        public StringInputStream(string input) : base(input)
        {
            Input = input;
        }
    }
}
