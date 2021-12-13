using Antlr4.Runtime.Tree;
using Qsi.Athena.Internal;
using Qsi.Diagnostics.Antlr;

namespace Qsi.Athena.Diagnostics
{
    public sealed class AthenaRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var (parser, result) = SqlParser.Parse(input, p => p.singleStatement());
            return (result, parser.RuleNames);
        }
    }
}
