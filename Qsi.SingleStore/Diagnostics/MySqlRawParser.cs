using System;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.SingleStore.Internal;

namespace Qsi.SingleStore.Diagnostics;

public class SingleStoreRawParser : AntlrRawParserBase
{
    private readonly int _version;
    private readonly bool _mariaDbCompatibility;

    public SingleStoreRawParser(Version version, bool mariaDBCompatibility)
    {
        _version = SingleStoreUtility.VersionToInt(version);
        _mariaDbCompatibility = mariaDBCompatibility;
    }

    protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
    {
        var parser = SingleStoreUtility.CreateParser(input, _version, _mariaDbCompatibility);
        return (parser.query(), parser.RuleNames);
    }
}
