using net.sf.jsqlparser.parser;
using Qsi.Diagnostics;

namespace Qsi.JSql.Diagnostics
{
    public sealed class JSqlRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            return new JSqlRawTree(CCJSqlParserUtil.parse(input));
        }
    }
}
