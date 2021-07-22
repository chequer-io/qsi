using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Impala.Utilities;

namespace Qsi.Impala.Diagnostics
{
    public class ImpalaRawParser : AntlrRawParserBase
    {
        private readonly ImpalaDialect _dialect;

        public ImpalaRawParser(ImpalaDialect dialect)
        {
            _dialect = dialect;
        }

        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var parser = ImpalaUtility.CreateParserInternal(input, _dialect);
            return (parser.root(), parser.RuleNames);
        }
    }
}
