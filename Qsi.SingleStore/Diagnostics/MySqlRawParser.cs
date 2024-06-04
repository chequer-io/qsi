using System;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.SingleStore.Internal;

namespace Qsi.SingleStore.Diagnostics;

public class SingleStoreRawParser : AntlrRawParserBase
{
    protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
    {
        var parser = SingleStoreUtility.CreateParser(input);
        return (parser.query(), parser.RuleNames);
    }
}
