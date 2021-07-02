using System.IO;
using Antlr4.Runtime;

namespace Qsi.Impala.Internal
{
    internal abstract class ImpalaBaseParser : Parser
    {
        private ImpalaBaseLexer _lexer;

        protected ImpalaBaseParser(ITokenStream input) : base(input)
        {
        }

        protected ImpalaBaseParser(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        public void Setup(ImpalaBaseLexer lexer)
        {
            _lexer = lexer;
        }
    }
}
