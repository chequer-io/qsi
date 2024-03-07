using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Trino.Internal;

namespace Qsi.Trino.Diagnostics;

public sealed class TrinoRawParser : AntlrRawParserBase
{
    protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
    {
        var (parser, result) = SqlParser.Parse(input, p => p.singleStatement());
        return (result, parser.RuleNames);
    }
}