using System.IO;
using Antlr4.Runtime;

namespace Qsi.PostgreSql.Antlr
{
    internal abstract class PlSqlParserBase : Parser
    {
        public bool IsVersion10 { get; set; }

        public bool IsVersion12 { get; set; } = true;

        protected PlSqlParserBase(ITokenStream input) : base(input)
        {
        }

        protected PlSqlParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }
    }
}
