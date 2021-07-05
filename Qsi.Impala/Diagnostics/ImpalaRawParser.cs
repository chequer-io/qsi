using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Impala.Internal;
using Qsi.Impala.Utilities;

namespace Qsi.Impala.Diagnostics
{
    public class ImpalaRawParser : AntlrRawParserBase
    {
        private readonly Version _verseion;

        public ImpalaRawParser(Version version)
        {
            _verseion = version;
        }

        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var parser = ImpalaUtility.CreateParserInternal(input, _verseion, Enumerable.Empty<string>());
            return (parser.stmt(), parser.RuleNames);
        }
    }
}
