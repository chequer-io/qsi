using Qsi.Diagnostics;
using Qsi.JSql.Internal;

namespace Qsi.JSql.Diagnostics
{
    public sealed class JSqlRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            return new JSqlRawTree(CCJSqlParserUtility.Parse(input));
        }
    }
}
