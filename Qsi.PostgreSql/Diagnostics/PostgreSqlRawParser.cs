using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.PostgreSql.Internal;

namespace Qsi.PostgreSql.Diagnostics
{
    public class PostgreSqlRawParser : AntlrRawParserBase
    {
        protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
        {
            var parser = PostgreSqlUtility.CreateParser(input);
            return (parser.root(), parser.RuleNames);
        }
    }
}
