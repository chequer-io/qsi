using net.sf.jsqlparser;
using net.sf.jsqlparser.parser;
using Qsi.Diagnostics;
using Qsi.JSql.Extensions;

namespace Qsi.JSql.Diagnostics
{
    public sealed class JSqlRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            try
            {
                return new JSqlRawTree(CCJSqlParserUtil.parse(input));
            }
            catch (JSQLParserException e)
            {
                throw e.AsSyntaxError();
            }
        }
    }
}
