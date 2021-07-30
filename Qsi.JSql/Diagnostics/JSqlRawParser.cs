using Qsi.Diagnostics;
using Qsi.JSql.Internal;

namespace Qsi.JSql.Diagnostics
{
    public class JSqlRawParser : IRawTreeParser
    {
        public virtual IRawTree Parse(string input)
        {
            return new JSqlRawTree(CCJSqlParserUtility.Parse(input));
        }
    }
}
